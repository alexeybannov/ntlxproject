using System;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantQuotaRowQuery
    {
        public static readonly TenantQuotaRowQuery All;

        internal string Path { get; private set; }
        internal bool ByPath { get; private set; }

        internal DateTime StartDateTime { get; private set; }
        internal DateTime EndDateTime { get; private set; }
        internal bool ByDateTime { get; private set; }
        internal string Tag { get; private set; }
        internal bool ByTag { get; private set; }

        public TenantQuotaRowQuery WithPath(string path)
        {
            ByPath = true;
            Path = path;
            return this;
        }

        public TenantQuotaRowQuery WithTag(string tag)
        {
            ByTag = true;
            Tag = tag;
            return this;
        }

        public TenantQuotaRowQuery WithDateTime(DateTime start, DateTime end)
        {
            ByDateTime = true;
            StartDateTime = start;
            EndDateTime = end;
            return this;
        }
    }
}