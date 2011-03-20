using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedAzService : IAzService
    {
        private const string KEY = "acl";
        private readonly IAzService service;
        private readonly ICache cache;
        private readonly object syncRoot;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }


        public CachedAzService(IAzService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();
            this.syncRoot = new object();

            CacheExpiration = TimeSpan.FromMinutes(5);
        }


        public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
        {
            var aces = cache.Get(KEY) as List<AzRecord>;
            if (aces == null)
            {
                lock (cache)
                {
                    aces = cache.Get(KEY) as List<AzRecord>;
                    if (aces == null)
                    {
                        aces = service.GetAces(Tenant.DEFAULT_TENANT, default(DateTime)).ToList();
                        cache.Insert(KEY, aces, DateTime.UtcNow.Add(CacheExpiration));
                    }
                }
            }

            lock (aces)
            {
                return aces.ToList();
            }
        }

        public AzRecord SaveAce(int tenant, AzRecord r)
        {
            r = service.SaveAce(tenant, r);
            var aces = cache.Get(KEY) as List<AzRecord>;
            if (aces != null)
            {
                lock (aces)
                {
                    aces.Remove(r);
                    aces.Add(r);
                }
            }
            return r;
        }

        public void RemoveAce(int tenant, AzRecord r)
        {
            service.RemoveAce(tenant, r);
            var aces = cache.Get(KEY) as List<AzRecord>;
            if (aces != null)
            {
                lock (aces) aces.Remove(r);
            }
        }
    }
}
