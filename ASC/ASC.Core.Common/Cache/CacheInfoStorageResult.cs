#region usings

using System;

#endregion

namespace ASC.Core.Common.Cache
{
    [Serializable]
    public class CacheInfoStorageResult
    {
        public CacheVersion StorageVersion { get; private set; }

        public CacheReaction Reaction { get; internal set; }

        public override string ToString()
        {
            return string.Format("Version = {0}, Reaction = {1}", StorageVersion, Reaction);
        }

        internal CacheInfoStorageResult(CacheVersion version)
            : this(version, CacheReaction.None)
        {
        }

        internal CacheInfoStorageResult(CacheVersion version, CacheReaction reaction)
        {
            if (version == null) throw new ArgumentNullException("version");
            StorageVersion = version;
            Reaction = reaction;
        }
    }
}