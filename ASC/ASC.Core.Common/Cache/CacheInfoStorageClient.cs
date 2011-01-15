#region usings

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;

#endregion

namespace ASC.Core.Common.Cache
{
    public class CacheInfoStorageClient
    {
        private readonly ICacheInfoStorage cacheInfoStorage;
        private readonly TimeSpan trustInterval;
        private readonly IDictionary<string, CacheRecord> caches;
        private readonly object syncRoot;

        public CacheInfoStorageClient(ICacheInfoStorage cacheInfoStorage)
            : this(cacheInfoStorage, TimeSpan.FromSeconds(5))
        {
        }

        public CacheInfoStorageClient(ICacheInfoStorage cacheInfoStorage, TimeSpan trustInterval)
        {
            if (cacheInfoStorage == null) throw new ArgumentNullException("cacheInfoStorage");
            this.cacheInfoStorage = cacheInfoStorage;
            syncRoot = new object();
            caches = new Dictionary<string, CacheRecord>();
            if (RemotingServices.IsObjectOutOfAppDomain(this.cacheInfoStorage))
            {
                this.trustInterval = trustInterval;
            }
            else
            {
                this.trustInterval = TimeSpan.Zero;
            }
        }

        public ICache<TKey, TValue> CreateCache<TKey, TValue>(string cacheId, SynchronizeFunc<TKey, TValue> syncFunc)
        {
            return CreateCache<TKey, TValue>(new CacheInfo(cacheId), syncFunc);
        }

        public ICache<TKey, TValue> CreateCache<TKey, TValue>(CacheInfo info, SynchronizeFunc<TKey, TValue> syncFunc)
        {
            lock (syncRoot)
            {
                return new Cache<TKey, TValue>(info.CacheId, syncFunc, CreateCacheController(info));
            }
        }

        public CacheController CreateCacheController(string cacheId)
        {
            return CreateCacheController(new CacheInfo(cacheId));
        }

        public CacheController CreateCacheController(CacheInfo info)
        {
            lock (syncRoot)
            {
                var controller = new CacheController(info.CacheId, this, trustInterval);
                cacheInfoStorage.RegisterCache(info);
                if (!caches.ContainsKey(info.CacheId))
                {
                    caches[info.CacheId] = new CacheRecord(info.CacheId);
                }
                caches[info.CacheId].AddController(controller);
                RebuildChildCaches(info);
                return controller;
            }
        }

        public CacheInfoStorageResult ValidateCache(CacheVersion version)
        {
            lock (syncRoot)
            {
                return cacheInfoStorage.ValidateCache(version);
            }
        }

        public CacheInfoStorageResult UpdateCache(CacheVersion version, CacheAction action)
        {
            lock (syncRoot)
            {
                ResetTrust(version.CacheId);
                return cacheInfoStorage.UpdateCache(version, action);
            }
        }

        public void ResetTrust(string cacheId)
        {
            lock (syncRoot)
            {
                if (string.IsNullOrEmpty(cacheId))
                {
                    foreach (CacheRecord record in caches.Values)
                    {
                        record.ResetTrust();
                    }
                }
                else
                {
                    CacheRecord record = caches[cacheId];
                    record.ResetTrust();
                    record.ChildCaches.ForEach(c => { caches[c].ResetTrust(); });
                }
            }
        }

        private void RebuildChildCaches(CacheInfo info)
        {
            foreach (string parentCache in info.GetParentCaches())
            {
                if (!caches.ContainsKey(parentCache))
                {
                    caches[parentCache] = new CacheRecord(parentCache);
                }
                if (!caches[parentCache].ChildCaches.Contains(info.CacheId))
                {
                    caches[parentCache].ChildCaches.Add(info.CacheId);
                }
            }
        }

        private class CacheRecord
        {
            public string CacheId { get; private set; }
            public List<string> ChildCaches { get; private set; }

            public void AddController(CacheController controller)
            {
                controllers.Add(controller);
            }

            private readonly List<CacheController> controllers;

            public CacheRecord(string cacheId)
            {
                CacheId = cacheId;
                ChildCaches = new List<string>();
                controllers = new List<CacheController>();
            }

            public void ResetTrust()
            {
                controllers.ForEach(c => c.ResetTrust());
            }
        }
    }
}