using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace ASC.Core
{
    class CacheHelper
    {
        private Cache cache;
        private IDictionary<string, DateTime> lasts;

        
        public TimeSpan Period
        {
            get;
            set;
        }

        public TimeSpan SligingExpiration
        {
            get;
            set;
        }


        public CacheHelper()
        {
            lasts = new Dictionary<string, DateTime>(1000);
            cache = HttpRuntime.Cache;
            Period = TimeSpan.FromSeconds(5);
            SligingExpiration = TimeSpan.FromMinutes(10);
        }


        public List<T> Get<T>(int tenant, DateTime from, Func<DateTime, IEnumerable<CacheItem<T>>> sync)
        {
            var key = Key<T>(tenant);
            var items = cache.Get(key) as List<CacheItem<T>>;
            var last = GetLast(key);

            if (items == null || Period < (last - DateTime.UtcNow).Duration())
            {
                SetLast(key);
                if (items == null) items = new List<CacheItem<T>>();
                foreach (var u in sync(last))
                {
                    items.Remove(u);
                    if (!u.Removed) items.Add(u);
                }
                cache.Insert(
                    key,
                    items,
                    null,
                    Cache.NoAbsoluteExpiration,
                    SligingExpiration,
                    CacheItemPriority.Default,
                    (k, o, r) => lasts.Remove(k));
            }

            return (from != default(DateTime) ? items.FindAll(u => u.ModifiedOn >= from) : items)
                .Select(i => i.Value)
                .ToList();
        }

        public void Reset<T>(int tenant)
        {
            lasts[Key<T>(tenant)] = DateTime.MinValue;
        }


        private string Key<T>(int tenant)
        {
            return string.Format("{0}{1}", typeof(T).Name, tenant);
        }

        private DateTime GetLast(string key)
        {
            DateTime last;
            lasts.TryGetValue(key, out last);
            return last;
        }

        private void SetLast(string key)
        {
            lasts[key] = DateTime.UtcNow;
        }
    }
}
