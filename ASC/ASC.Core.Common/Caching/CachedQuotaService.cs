using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedQuotaService : IQuotaService
    {
        private const string KEY_QUOTAS = "quotas";
        private const string KEY_QUOTA_ROWS = "quotarows";
        private readonly IQuotaService service;
        private readonly ICache cache;
        private readonly TrustInterval interval;

        private readonly object syncQuotas;
        private int syncQuotaRows;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }


        public CachedQuotaService(IQuotaService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();
            this.interval = new TrustInterval();
            this.syncQuotas = new object();
            this.syncQuotaRows = 0;

            CacheExpiration = TimeSpan.FromMinutes(5);
        }


        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            var quotas = GetTenantQuotasInternal();
            lock (quotas)
            {
                return quotas.Values.ToList();
            }
        }

        public TenantQuota GetTenantQuota(int tenant, string name)
        {
            var quotas = GetTenantQuotasInternal();
            lock (quotas)
            {
                TenantQuota q;
                quotas.TryGetValue(tenant.ToString() + name, out q);
                if (q == null) quotas.TryGetValue(Tenant.DEFAULT_TENANT + name, out q);
                return q;
            }
        }

        public void SetTenantQuota(TenantQuota quota)
        {
            service.SetTenantQuota(quota);
            var quotas = GetTenantQuotasInternal();
            lock (quotas) quotas.Remove(quota.Tenant.ToString() + quota.Name);
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            service.SetTenantQuotaRow(tenant, name, row, exchange);
            interval.Stop();
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            if (Interlocked.CompareExchange(ref syncQuotaRows, 1, 0) == 0)
            {
                try
                {
                    var rows = cache.Get(KEY_QUOTA_ROWS) as Dictionary<string, List<TenantQuotaRow>>;
                    if (rows == null || interval.Expired)
                    {
                        var date = interval.StartTime;
                        interval.Start(CacheExpiration);
                        if (rows == null) rows = new Dictionary<string, List<TenantQuotaRow>>();

                        rows = service.FindTenantQuotaRows(Tenant.DEFAULT_TENANT, null, new TenantQuotaRowQuery().WithLastModified(date))
                            .GroupBy(r => r.Tenant.ToString() + r.Name)
                            .ToDictionary(g => g.Key, g => g.ToList());

                        cache.Insert(KEY_QUOTA_ROWS, rows, CacheExpiration);
                    }
                }
                finally
                {
                    syncQuotaRows = 0;
                }
            }
            var quotaRows = cache.Get(KEY_QUOTA_ROWS) as IDictionary<string, List<TenantQuotaRow>>;
            if (quotaRows == null) return new TenantQuotaRow[0];

            lock (quotaRows)
            {
                var list = quotaRows.ContainsKey(tenant.ToString() + name) ?
                    quotaRows[tenant.ToString() + name] :
                    new List<TenantQuotaRow>();
                return list.Where(r => !string.IsNullOrEmpty(query.Path) ? query.Path == r.Path : true);
            }
        }


        private IDictionary<string, TenantQuota> GetTenantQuotasInternal()
        {
            var quotas = cache.Get(KEY_QUOTAS) as IDictionary<string, TenantQuota>;
            if (quotas == null)
            {
                lock (syncQuotas)
                {
                    quotas = cache.Get(KEY_QUOTAS) as IDictionary<string, TenantQuota>;
                    if (quotas == null)
                    {
                        quotas = new Dictionary<string, TenantQuota>();
                        foreach (var q in service.GetTenantQuotas())
                        {
                            quotas[q.Tenant.ToString() + q.Name] = q;
                        }
                        cache.Insert(KEY_QUOTAS, quotas, DateTime.UtcNow.Add(CacheExpiration));
                    }
                }
            }
            return quotas;
        }
    }
}
