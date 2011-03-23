﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedTenantService : ITenantService
    {
        private const string KEY = "tenants";
        private readonly ITenantService service;
        private readonly ICache cache;
        private readonly TrustInterval interval;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }

        public TimeSpan DbExpiration
        {
            get;
            set;
        }

        public TimeSpan SettingsExpiration
        {
            get;
            set;
        }


        public CachedTenantService(ITenantService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            cache = new AspCache();
            interval = new TrustInterval();

            CacheExpiration = TimeSpan.FromHours(2);
            DbExpiration = TimeSpan.FromMinutes(10);
            SettingsExpiration = TimeSpan.FromMinutes(1);
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
            lock (cache)
            {
                var fromdb = false;
                var tenants = GetTenantStore(ref fromdb);
                return (from != default(DateTime) ? tenants : tenants.Where(t => t.LastModified >= from)).ToList();
            }
        }

        public Tenant GetTenant(int id)
        {
            lock (cache)
            {
                var fromdb = false;
                var tenants = GetTenantStore(ref fromdb);
                var t = tenants.Get(id);

                if (!fromdb && t == null)
                {
                    fromdb = true;
                    tenants = GetTenantStore(ref fromdb);
                    t = tenants.Get(id);
                }

                return t;
            }
        }

        public Tenant GetTenant(string domain)
        {
            lock (cache)
            {
                var fromdb = false;
                var tenants = GetTenantStore(ref fromdb);
                var t = tenants.Get(domain);

                if (!fromdb && t == null)
                {
                    fromdb = true;
                    tenants = GetTenantStore(ref fromdb);
                    t = tenants.Get(domain);
                }

                return t;
            }
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            tenant = service.SaveTenant(tenant);
            lock (cache)
            {
                var tenants = cache.Get(KEY) as TenantStore;
                if (tenants != null) tenants.Insert(tenant);
            }
            return tenant;
        }

        public void RemoveTenant(int id)
        {
            service.RemoveTenant(id);
            lock (cache)
            {
                var tenants = cache.Get(KEY) as TenantStore;
                if (tenants != null) tenants.Remove(id);
            }
        }

        public byte[] GetTenantSettings(int tenant, string key)
        {
            var cacheKey = string.Format("settings/{0}/{1}", tenant, key);
            var data = cache.Get(cacheKey) as byte[] ?? service.GetTenantSettings(tenant, key);
            cache.Insert(cacheKey, data ?? new byte[0], SettingsExpiration);
            return data.Length == 0 ? null : data;
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            service.SetTenantSettings(tenant, key, data);
            cache.Insert(string.Format("settings/{0}/{1}", tenant, key), data ?? new byte[0], SettingsExpiration);
        }


        private TenantStore GetTenantStore(ref bool fromdb)
        {
            var store = cache.Get(KEY) as TenantStore;
            if (store == null || interval.Expired || fromdb)
            {
                fromdb = true;
                var date = interval.StartTime;
                interval.Start(DbExpiration);

                var tenants = service.GetTenants(date);
                if (store == null) cache.Insert(KEY, store = new TenantStore(), CacheExpiration);

                foreach (var t in tenants)
                {
                    store.Insert(t);
                }
            }
            return store;
        }


        private class TenantStore : IEnumerable<Tenant>
        {
            private readonly Dictionary<int, Tenant> byId = new Dictionary<int, Tenant>();
            private readonly Dictionary<string, Tenant> byDomain = new Dictionary<string, Tenant>();


            public Tenant Get(int id)
            {
                Tenant t;
                byId.TryGetValue(id, out t);
                return t;
            }

            public Tenant Get(string domain)
            {
                if (string.IsNullOrEmpty(domain)) return null;

                Tenant t;
                byDomain.TryGetValue(domain, out t);
                return t;
            }

            public void Insert(Tenant t)
            {
                if (t == null) return;
                Remove(t.TenantId);

                byId[t.TenantId] = t;
                byDomain[t.TenantAlias] = t;
                if (!string.IsNullOrEmpty(t.MappedDomain)) byDomain[t.MappedDomain] = t;
            }

            public void Remove(int id)
            {
                var t = Get(id);
                if (t != null)
                {
                    byId.Remove(id);
                    byDomain.Remove(t.TenantAlias);
                    if (!string.IsNullOrEmpty(t.MappedDomain)) byDomain.Remove(t.MappedDomain);
                }
            }


            public IEnumerator<Tenant> GetEnumerator()
            {
                return byId.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
