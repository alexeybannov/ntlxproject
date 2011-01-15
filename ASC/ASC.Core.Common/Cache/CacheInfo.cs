#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Cache
{
    [Serializable]
    public class CacheInfo
    {
        public string CacheId { get; private set; }
        private readonly IList<string> parentCaches;
        private readonly IDictionary<string, CacheReaction> onSaveOrUpdateReaction;
        private readonly IDictionary<string, CacheReaction> onRemoveReaction;
        private readonly IDictionary<string, CacheReaction> onFlushReaction;

        public CacheInfo(string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId)) throw new ArgumentException("cacheId");
            CacheId = cacheId;
            parentCaches = new List<string>();
            onSaveOrUpdateReaction = new Dictionary<string, CacheReaction>();
            onRemoveReaction = new Dictionary<string, CacheReaction>();
            onFlushReaction = new Dictionary<string, CacheReaction>();
        }

        public void AddParentCache(string parentCacheId)
        {
            AddParentCache(parentCacheId, CacheReaction.None, CacheReaction.Synchronize, CacheReaction.Synchronize);
        }

        public void AddParentCache(string parentCacheId, CacheReaction onSaveOrUpdate, CacheReaction onRemove,
                                   CacheReaction onFlush)
        {
            if (!parentCaches.Contains(parentCacheId)) parentCaches.Add(parentCacheId);
            onSaveOrUpdateReaction[parentCacheId] = onSaveOrUpdate;
            onRemoveReaction[parentCacheId] = onRemove;
            onFlushReaction[parentCacheId] = onFlush;
        }

        public void RemoveParentCache(string parentCacheId)
        {
            parentCaches.Remove(parentCacheId);
            onSaveOrUpdateReaction.Remove(parentCacheId);
            onRemoveReaction.Remove(parentCacheId);
            onFlushReaction.Remove(parentCacheId);
        }

        public ICollection<string> GetParentCaches()
        {
            return new List<string>(parentCaches);
        }

        public CacheReaction GetReaction(string cacheId, CacheAction action)
        {
            switch (action)
            {
                case CacheAction.SaveOrUpdate:
                    return onSaveOrUpdateReaction[cacheId];
                case CacheAction.Remove:
                    return onRemoveReaction[cacheId];
                case CacheAction.Flush:
                    return onFlushReaction[cacheId];
            }
            throw new ArgumentOutOfRangeException("action");
        }

        internal void Merge(CacheInfo info)
        {
            if (CacheId != info.CacheId) throw new InvalidOperationException("Caches not equals.");
            foreach (string parent in info.GetParentCaches())
            {
                if (!parentCaches.Contains(parent))
                {
                    AddParentCache(
                        parent,
                        info.GetReaction(parent, CacheAction.SaveOrUpdate),
                        info.GetReaction(parent, CacheAction.Remove),
                        info.GetReaction(parent, CacheAction.Flush)
                        );
                }
            }
        }
    }
}