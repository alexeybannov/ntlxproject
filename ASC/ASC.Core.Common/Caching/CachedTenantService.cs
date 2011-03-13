using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedTenantService : ITenantService
    {
        private const string KEY_BY_ID = "tenants";
        private const string KEY_BY_DOMAIN = "tenants2";

        private readonly object syncRoot;
        private readonly ITenantService service;
        private readonly ICache cache;
        private readonly TrustInterval interval;


        public TimeSpan SettingsExpiration
        {
            get;
            set;
        }


        public CachedTenantService(ITenantService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.syncRoot = new object();
            this.service = service;
            this.cache = new AspCache();
            this.interval = new TrustInterval();
            this.SettingsExpiration = TimeSpan.FromMinutes(1);
        }


        public void ValidateDomain(string domain)
        {
            service.ValidateDomain(domain);
        }

        public IEnumerable<Tenant> GetTenants(string login, string password)
        {
            return service.GetTenants(login, password);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from)
        {
            lock (syncRoot)
            {
                var tenants = cache.Get(KEY_BY_ID) as IDictionary<int, Tenant>;
                if (tenants == null)
                {
                    InvalidateCache();
                    GetChangesFromDb();
                    tenants = cache.Get(KEY_BY_ID) as IDictionary<int, Tenant>;
                }
                return tenants.Values;
            }
        }

        private void GetChangesFromDb()
        {
            throw new NotImplementedException();
        }

        public Tenant GetTenant(int id)
        {
            var tenants = cache.Get(KEY_BY_ID) as IDictionary<int, Tenant>;
            if (tenants == null)
            {
                InvalidateCache();
            }
            return service.GetTenant(id);
        }

        public Tenant GetTenant(string domain)
        {
            return service.GetTenant(domain);
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            tenant = service.SaveTenant(tenant);
            lock (cache)
            {
                var tenants = cache.Get(KEY_BY_ID) as IDictionary<int, Tenant>;
                if (tenants != null)
                {
                    tenants[tenant.TenantId] = tenant;
                }

                var tenants2 = cache.Get(KEY_BY_DOMAIN) as IDictionary<string, Tenant>;
                if (tenants2 != null)
                {
                    //remove first if alias or mappeddomain has been changes
                    tenants2.Remove(tenant.TenantAlias);
                    if (!string.IsNullOrEmpty(tenant.MappedDomain)) tenants2.Remove(tenant.MappedDomain);

                    tenants2[tenant.TenantAlias] = tenant;
                    if (!string.IsNullOrEmpty(tenant.MappedDomain)) tenants2[tenant.MappedDomain] = tenant;
                }
            }
            return tenant;
        }

        public void RemoveTenant(int id)
        {
            service.RemoveTenant(id);
            lock (cache)
            {
                var tenants = cache.Get(KEY_BY_ID) as IDictionary<int, Tenant>;
                if (tenants != null)
                {
                    Tenant t;
                    tenants.TryGetValue(id, out t);
                    if (t != null)
                    {
                        tenants.Remove(id);
                        var tenants2 = cache.Get(KEY_BY_DOMAIN) as IDictionary<string, Tenant>;
                        if (tenants2 != null)
                        {
                            tenants2.Remove(t.TenantAlias);
                            if (!string.IsNullOrEmpty(t.MappedDomain)) tenants2.Remove(t.MappedDomain);
                        }
                    }
                }
            }
        }

        public byte[] GetTenantSettings(int tenant, string key)
        {
            var cacheKey = string.Format("settings/{0}/key", tenant, key);
            var data = cache.Get(cacheKey) as byte[] ?? service.GetTenantSettings(tenant, key);
            cache.Insert(cacheKey, data ?? new byte[0], SettingsExpiration);
            return data.Length == 0 ? null : data;
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            service.SetTenantSettings(tenant, key, data);
            cache.Insert(string.Format("settings/{0}/key", tenant, key), data ?? new byte[0], SettingsExpiration);
        }


        private void InvalidateCache()
        {
            interval.Stop();
        }
    }
}
