#region usings

using System;

#endregion

namespace ASC.Core.Common.Cache
{
    [Serializable]
    public class CacheVersion : IEquatable<CacheVersion>, ICloneable
    {
        public string CacheId { get; private set; }

        public Guid Session { get; private set; }

        public long Version { get; private set; }

        public CacheVersion(string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId)) throw new ArgumentException("cacheId");
            CacheId = cacheId;
            Session = Guid.NewGuid();
            Version = 0;
        }

        internal void Increase()
        {
            Version++;
        }

        internal void Reset()
        {
            Version = 0;
        }

        public override int GetHashCode()
        {
            return (Session.GetHashCode() ^ Version.GetHashCode()*CacheId.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CacheVersion);
        }

        public override string ToString()
        {
            return string.Format("CacheId = {0}, Version = {1}", CacheId, Version);
        }

        #region IEquatable<CacheVersion> Members

        public bool Equals(CacheVersion version)
        {
            return
                version != null &&
                Session == version.Session &&
                Version == version.Version &&
                CacheId == version.CacheId;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}