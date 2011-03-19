using System;
using System.Web;
using System.Web.Caching;

namespace ASC.Core.Caching
{
    class AspCache : ICache
    {
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Insert(string key, object value, TimeSpan sligingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, sligingExpiration);
        }

        public void Insert(string key, object value, DateTime absolutExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, absolutExpiration, Cache.NoSlidingExpiration);
        }

        public object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }
    }
}
