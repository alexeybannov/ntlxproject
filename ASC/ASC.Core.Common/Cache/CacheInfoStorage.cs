#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Cache
{
    public class CacheInfoStorage : ICacheInfoStorage
    {
        private readonly IDictionary<string, CacheRecord> caches;
        private readonly object syncRoot;

        public CacheInfoStorage()
        {
            syncRoot = new object();
            caches = new Dictionary<string, CacheRecord>();
        }

        #region ICacheInfoStorage Members

        public void RegisterCache(CacheInfo info)
        {
            lock (syncRoot)
            {
                if (!caches.ContainsKey(info.CacheId))
                {
                    caches.Add(info.CacheId, new CacheRecord(info));
                }
                else
                {
                    caches[info.CacheId].Info.Merge(info);
                }
            }
        }

        public void UnregisterCache(Guid cacheId)
        {
            throw new NotImplementedException();
        }

        public CacheInfoStorageResult ValidateCache(CacheVersion version)
        {
            lock (syncRoot)
            {
                if (caches.ContainsKey(version.CacheId))
                {
                    CacheVersion storageVersion = caches[version.CacheId].Version;
                    var result = new CacheInfoStorageResult(
                        (CacheVersion) storageVersion.Clone(),
                        storageVersion.Equals(version) ? CacheReaction.None : CacheReaction.Synchronize
                        );
                    return result;
                }
                throw new InvalidOperationException("Cache not registered.");
            }
        }

        public CacheInfoStorageResult UpdateCache(CacheVersion version, CacheAction action)
        {
            lock (syncRoot)
            {
                if (caches.ContainsKey(version.CacheId))
                {
                    CacheVersion storageVersion = caches[version.CacheId].Version;
                    CacheReaction reaction = CacheReaction.Synchronize;
                    if (storageVersion.Equals(version) && action == CacheAction.SaveOrUpdate)
                    {
                        reaction = CacheReaction.None;
                    }

                    storageVersion.Increase();
                    var result = new CacheInfoStorageResult((CacheVersion) storageVersion.Clone(), reaction);

                    UpdateChildCaches(version.CacheId, action);
                    return result;
                }
                throw new InvalidOperationException("Cache not registered.");
            }
        }

        #endregion

        private void UpdateChildCaches(string cacheId, CacheAction action)
        {
            foreach (CacheRecord r in caches.Values)
            {
                if (r.Info.GetParentCaches().Contains(cacheId))
                {
                    CacheReaction reaction = r.Info.GetReaction(cacheId, action);
                    if (reaction == CacheReaction.Synchronize)
                    {
                        r.Version.Increase();
                    }
                }
            }
        }

        private class CacheRecord
        {
            public string CacheId
            {
                get { return Info.CacheId; }
            }

            public CacheVersion Version { get; private set; }
            public CacheInfo Info { get; private set; }

            public CacheRecord(CacheInfo info)
            {
                Info = info;
                Version = new CacheVersion(CacheId);

                Version.Increase();
            }

            public override bool Equals(object obj)
            {
                var r = obj as CacheRecord;
                return r != null && CacheId == r.CacheId;
            }

            public override int GetHashCode()
            {
                return CacheId.GetHashCode();
            }
        }
    }
}