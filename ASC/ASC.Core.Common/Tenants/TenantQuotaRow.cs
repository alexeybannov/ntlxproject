using System;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantQuotaRow
    {
        public TenantQuotaRow(string path, long counter, string tag)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            Path = path;
            Counter = counter;
            Tag = tag;
        }

        public string Path { get; set; }

        public DateTime DateTime { get; set; }

        public long Counter { get; set; }

        public string Tag { get; set; }

        public int Tenant { get; set; }

        public string Name { get; set; }
    }
}