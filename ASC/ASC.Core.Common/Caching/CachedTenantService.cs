using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedTenantService : ITenantService
    {
        private readonly ITenantService service;
        private readonly ICache cache;


        public TimeSpan SettingsExpiration
        {
            get;
            set;
        }


        public CachedTenantService(ITenantService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();
            this.SettingsExpiration = TimeSpan.FromMinutes(1);
        }


        public void ValidateDomain(string domain)
        {
            service.ValidateDomain(domain);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from)
        {
            return service.GetTenants(from);
        }

        public IEnumerable<Tenant> GetTenants(string login, string password)
        {
            return service.GetTenants(login, password);
        }

        public Tenant GetTenant(int id)
        {
            return service.GetTenant(id);
        }

        public Tenant GetTenant(string domain)
        {
            return service.GetTenant(domain);
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            tenant = service.SaveTenant(tenant);
            return tenant;
        }

        public void RemoveTenant(int id)
        {
            service.RemoveTenant(id);
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



    }
}
