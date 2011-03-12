using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbQuotaService : DbBaseService, IQuotaService
    {
        public DbQuotaService(ConnectionStringSettings connectionString)
            : base(connectionString, null)
        {
        }


        public TenantQuota GetTenantQuota(int tenant, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var q = new SqlQuery("tenants_quota")
                .Select("quota")
                .Where("name", name)
                .Where(Exp.Eq("tenant", tenant) | Exp.Eq("tenant", Tenant.DEFAULT_TENANT))
                .OrderBy("tenant", false)
                .SetMaxResults(1);
            var quotasString = ExecScalar<string>(q);
            return !string.IsNullOrEmpty(quotasString) ? new TenantQuota(quotasString) : null;
        }

        public void SetTenantQuota(int tenant, string name, TenantQuota quota)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            if (quota != null)
            {
                ExecNonQuery(new SqlInsert("tenants_quota", true)
                    .InColumnValue("tenant", tenant)
                    .InColumnValue("name", name)
                    .InColumnValue("quota", quota.QuotasString));
            }
            else
            {
                ExecNonQuery(new SqlDelete("tenants_quota")
                    .Where("name", name)
                    .Where("tenant", tenant));
            }
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (row == null) throw new ArgumentNullException("row");

            ExecAction(db =>
            {
                var counter = db.ExecScalar<long>(
                    new SqlQuery("tenants_quotarow")
                    .Select("counter")
                    .Where("tenant", tenant)
                    .Where("name", name)
                    .Where("path", row.Path)
                    .Where("datetime", row.DateTime));

                db.ExecNonQuery(
                    new SqlInsert("tenants_quotarow", true)
                    .InColumnValue("tenant", tenant)
                    .InColumnValue("name", name)
                    .InColumnValue("path", row.Path)
                    .InColumnValue("datetime", row.DateTime)
                    .InColumnValue("counter", exchange ? counter + row.Counter : row.Counter)
                    .InColumnValue("tag", row.Tag));
            });
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var q = new SqlQuery("tenants_quotarow").Select("path", "datetime", "counter", "tag");
            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("tenant", tenant);
            }
            if (query != TenantQuotaRowQuery.All)
            {
                if (query.ByPath) q.Where("path", query.Path);
                if (query.ByDateTime) q.Where(Exp.Between("datetime", query.StartDateTime, query.EndDateTime));
                if (query.ByTag) q.Where("tag", query.Tag);
            }

            return ExecList(q)
                .ConvertAll(r => new TenantQuotaRow((string)r[0], Convert.ToInt64(r[2]), (string)r[3]) { DateTime = (DateTime)r[1] });
        }
    }
}
