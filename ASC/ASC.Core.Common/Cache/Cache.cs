using System;
using System.Collections.Generic;


namespace ASC.Core.Common.Cache
{
    public class Cache<TKey, TValue> : ICache<TKey, TValue>
    {
        private IDictionary<TKey, TValue> innerCache;

        private readonly CacheController controller;

        private readonly SynchronizeFunc<TKey, TValue> syncFunc;

        private readonly object syncRoot;


        public string Id
        {
            get;
            private set;
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (syncRoot)
                {
                    InnerSynchronize();
                    return innerCache[key];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    innerCache[key] = value;
                    AddCacheAction(CacheAction.SaveOrUpdate);
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (syncRoot)
                {
                    InnerSynchronize();
                    return innerCache.Keys;
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (syncRoot)
                {
                    InnerSynchronize();
                    return innerCache.Values;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    InnerSynchronize();
                    return innerCache.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return innerCache.IsReadOnly; }
        }


        internal Cache(string cacheId, SynchronizeFunc<TKey, TValue> syncFunc, CacheController controller)
        {
            if (string.IsNullOrEmpty(cacheId)) throw new ArgumentException("cacheId");
            if (syncFunc == null) throw new ArgumentNullException("syncFunc");
            if (controller == null) throw new ArgumentNullException("controller");

            this.syncRoot = new object();
            this.innerCache = new Dictionary<TKey, TValue>();
            this.Id = cacheId;
            this.syncFunc = syncFunc;
            this.controller = controller;
        }


        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Remove(TKey key)
        {
            lock (syncRoot)
            {
                var result = innerCache.Remove(key);
                AddCacheAction(CacheAction.Remove);
                return result;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                var result = innerCache.Remove(item);
                AddCacheAction(CacheAction.Remove);
                return result;
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                innerCache.Clear();
                AddCacheAction(CacheAction.Flush);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (syncRoot)
            {
                InnerSynchronize();
                return innerCache.ContainsKey(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (syncRoot)
            {
                InnerSynchronize();
                return innerCache.Contains(item);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (syncRoot)
            {
                InnerSynchronize();
                return innerCache.TryGetValue(key, out value);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (syncRoot)
            {
                InnerSynchronize();
                return innerCache.GetEnumerator();
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            lock (syncRoot)
            {
                InnerSynchronize();
                innerCache.CopyTo(array, index);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Synchronize()
        {
            lock (syncRoot)
            {
                InnerSynchronize(true);
            }
        }

        public void Flush()
        {
            lock (syncRoot)
            {
                AddCacheAction(CacheAction.Flush);
            }
        }


        private void InnerSynchronize()
        {
            InnerSynchronize(false);
        }

        private void InnerSynchronize(bool immediate)
        {
            if (immediate)
            {
                controller.ResetResult();
            }
            var result = controller.ValidateCache();
            ProcessResult(result);
        }

        private void AddCacheAction(CacheAction action)
        {
            var result = controller.UpdateCache(action);
            ProcessResult(result);
        }

        private void ProcessResult(CacheInfoStorageResult result)
        {
            if (result.Reaction == CacheReaction.None)
            {
                return;
            }
            else if (result.Reaction == CacheReaction.Synchronize)
            {
                IDictionary<TKey, TValue> values = null;
                try
                {
                    values = syncFunc();
                }
                catch
                {
                    controller.ResetResult();
                    return;
                }
                if (values == null)
                {
                    throw new InvalidOperationException("Cache synchronization function return null.");
                }
                innerCache = new Dictionary<TKey, TValue>(values);
            }
            else
            {
                throw new InvalidOperationException("Unknown cache reaction.");
            }
        }
    }
}