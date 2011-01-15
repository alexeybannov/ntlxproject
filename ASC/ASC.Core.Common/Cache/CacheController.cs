#region usings

using System;

#endregion

namespace ASC.Core.Common.Cache
{
    public class CacheController
    {
        private readonly CacheInfoStorageClient client;
        private readonly TimeSpan trustInterval;
        private DateTime lastInvoke;
        private CacheInfoStorageResult lastResult;

        internal CacheController(string cacheId, CacheInfoStorageClient client, TimeSpan trustInterval)
        {
            if (client == null) throw new ArgumentNullException("client");
            this.client = client;
            this.trustInterval = trustInterval;
            lastInvoke = DateTime.MinValue;
            lastResult = new CacheInfoStorageResult(new CacheVersion(cacheId));
        }

        public CacheVersion GetCacheVersion()
        {
            if (IsNotTrust())
            {
                lastResult = client.ValidateCache(lastResult.StorageVersion);
                SetTrust();
            }
            return lastResult.StorageVersion;
        }

        public CacheInfoStorageResult ValidateCache()
        {
            CacheInfoStorageResult result = lastResult;
            if (IsNotTrust())
            {
                result = client.ValidateCache(lastResult.StorageVersion);
                SetTrust();
            }
            lastResult = new CacheInfoStorageResult(result.StorageVersion, CacheReaction.None);
            return result;
        }

        public CacheInfoStorageResult UpdateCache(CacheAction action)
        {
            CacheInfoStorageResult result = client.UpdateCache(lastResult.StorageVersion, action);
            SetTrust();
            lastResult = new CacheInfoStorageResult(result.StorageVersion, CacheReaction.None);
            return result;
        }

        public void ResetTrust()
        {
            lastInvoke = DateTime.MinValue;
        }

        public void ResetResult()
        {
            lastResult.StorageVersion.Reset();
            ResetTrust();
        }

        private bool IsNotTrust()
        {
            return trustInterval <= (DateTime.UtcNow - lastInvoke);
        }

        private void SetTrust()
        {
            lastInvoke = DateTime.UtcNow;
        }
    }
}