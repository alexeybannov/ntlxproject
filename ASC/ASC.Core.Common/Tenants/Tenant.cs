using System;
using System.Collections.Generic;
using System.Globalization;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class Tenant : IEquatable<Tenant>
    {
        public const int DEFAULT_TENANT = -1;

        public Tenant()
        {
            TenantId = DEFAULT_TENANT;
            TimeZone = TimeZoneInfo.Local;
            Language = CultureInfo.CurrentCulture.Name;
            TrustedDomains = new List<string>();
            MappedDomains = new List<string>();
            TrustedDomainsEnabled = true;
            CreatedDateTime = DateTime.UtcNow;
            Status = TenantStatus.Active;
            StatusChangeDate = DateTime.UtcNow;
        }

        public Tenant(string alias)
            : this()
        {
            TenantAlias = alias.ToLowerInvariant();
        }

        internal Tenant(int id, string alias)
            : this(alias)
        {
            TenantId = id;
        }

        public int TenantId { get; internal set; }

        public string TenantAlias { get; set; }

        public string TenantDomain { get; set; }

        public string Name { get; set; }

        public string Language { get; set; }

        public TimeZoneInfo TimeZone { get; set; }

        public List<string> TrustedDomains { get; private set; }

        public bool TrustedDomainsEnabled { get; set; }

        public string OwnerEMail { get; set; }

        public string OwnerName { get; set; }

        public DateTime CreatedDateTime { get; internal set; }

        public CultureInfo GetCulture()
        {
            return new CultureInfo(Language);
        }

        public string MappedDomain { get; set; }

        public TenantStatus Status { get; internal set; }

        public DateTime StatusChangeDate { get; internal set; }

        public void SetStatus(TenantStatus status)
        {
            Status = status;
            StatusChangeDate = DateTime.UtcNow;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Tenant)) return false;
            return Equals((Tenant) obj);
        }

        public bool Equals(Tenant other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.TenantId == TenantId;
        }

        public override int GetHashCode()
        {
            return TenantId;
        }

        public override string ToString()
        {
            return TenantDomain ?? TenantAlias;
        }


        internal string GetTrustedDomains()
        {
            TrustedDomains.RemoveAll(d => string.IsNullOrEmpty(d));
            if (TrustedDomains.Count == 0) return null;
            return string.Join("|", TrustedDomains.ToArray());
        }

        internal void SetTrustedDomains(string trustedDomains)
        {
            if (string.IsNullOrEmpty(trustedDomains))
            {
                TrustedDomains.Clear();
            }
            else
            {
                TrustedDomains.AddRange(trustedDomains.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        internal List<string> MappedDomains { get; private set; }
    }

    public enum TenantStatus
    {
        Active = 0,
        Suspended = 1,
        RemovePending = 2
    }
}