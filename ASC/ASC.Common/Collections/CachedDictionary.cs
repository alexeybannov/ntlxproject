#region usings

using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;

#endregion

namespace ASC.Common.Collections
{
    public class CachedDictionary<T>
    {
        private readonly DateTime _absoluteExpiration;
        private readonly string _baseKey;
        private readonly Func<T, bool> _cacheCodition;
        private readonly TimeSpan _slidingExpiration;
        private Guid _clearId = Guid.NewGuid();

        public CachedDictionary(string baseKey, DateTime absoluteExpiration, TimeSpan slidingExpiration,
                                Func<T, bool> cacheCodition)
        {
            if (cacheCodition == null) throw new ArgumentNullException("cacheCodition");
            _baseKey = baseKey;
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
            _cacheCodition = cacheCodition;
            InsertRootKey();
        }

        public CachedDictionary(string baseKey)
            : this(baseKey, (x) => true)
        {
        }

        public CachedDictionary(string baseKey, Func<T, bool> cacheCodition)
            : this(baseKey, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, cacheCodition)
        {
        }

        public T this[string key]
        {
            get { return Get(key); }
        }

        public T this[Func<T> @default]
        {
            get { return Get(@default); }
        }

        private void InsertRootKey()
        {
            if (HttpRuntime.Cache.Get(_baseKey) == null)
            {
                HttpRuntime.Cache.Insert(_baseKey, _clearId, null, _absoluteExpiration, _slidingExpiration,
                                         CacheItemPriority.NotRemovable, null);
            }
        }

        public void Clear()
        {
            _clearId = Guid.NewGuid();
            InsertRootKey();
        }

        public T Get(string key)
        {
            return Get(key, null);
        }

        public T Get(string key, Func<T> defaults)
        {
            string fullKey = BuildKey(key);
            object objectCache = HttpRuntime.Cache.Get(fullKey);
            if (!Equals(objectCache, default(T)) && objectCache is T)
            {
                return (T) objectCache;
            }
            if (defaults != null)
            {
                Debug.Print("Cache miss. key:{0}", key);

                T newValue = defaults();
                if (_cacheCodition(newValue))
                {
                    Add(key, newValue);
                }
                return newValue;
            }
            return default(T);
        }

        public void Add(string key, T newValue)
        {
            HttpRuntime.Cache.Insert(BuildKey(key), newValue, new CacheDependency(null, new[] {_baseKey}),
                                     _absoluteExpiration, _slidingExpiration,
                                     CacheItemPriority.NotRemovable, null);
        }

        public bool HasItem(string key)
        {
            return !Equals(Get(key), default(T));
        }

        private string BuildKey(string key)
        {
            return string.Format("{0}-{1}", _baseKey, key);
        }

        public T Get(Func<T> @default)
        {
            string key = string.Format("func {0} {2}.{1}({3})", @default.Method.ReturnType, @default.Method.Name,
                                       @default.Method.DeclaringType.FullName,
                                       string.Join(",",
                                                   @default.Method.GetGenericArguments().Select(x => x.FullName).ToArray
                                                       ()));
            return Get(key, @default);
        }
    }
}