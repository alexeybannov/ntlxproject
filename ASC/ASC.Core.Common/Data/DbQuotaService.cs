using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbQuotaService : DbBaseService, IQuotaService
    {
        public DbQuotaService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {
        }


        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            var q = new SqlQuery("tenants_quota").Select("quota", "tenant", "name");
            return ExecList(q)
                .ConvertAll(r => new TenantQuota((string)r[0]) { Tenant = Convert.ToInt32(r[1]), Name = (string)r[2] });
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
            var xml = ExecScalar<string>(q);
            return !string.IsNullOrEmpty(xml) ? new TenantQuota(xml) { Tenant = tenant, Name = name } : null;
        }

        public void SetTenantQuota(TenantQuota quota)
        {
            if (quota == null) throw new ArgumentNullException("quota");

            var i = !string.IsNullOrEmpty(quota.Xml) ?
                (ISqlInstruction)Insert("tenants_quota", quota.Tenant).InColumnValue("name", quota.Name).InColumnValue("quota", quota.Xml) :
                (ISqlInstruction)Delete("tenants_quota", quota.Tenant).Where("name", quota.Name);

            ExecNonQuery(i);
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (row == null) throw new ArgumentNullException("row");

            row.Tenant = tenant;
            row.Name = name;

            ExecAction(db =>
            {
                var counter = db.ExecScalar<long>(Query("tenants_quotarow", tenant)
                    .Select("counter")
                    .Where("name", name)
                    .Where("path", row.Path)
                    .Where("datetime", row.DateTime));

                db.ExecNonQuery(Insert("tenants_quotarow", tenant)
                    .InColumnValue("name", name)
                    .InColumnValue("path", row.Path)
                    .InColumnValue("datetime", row.DateTime)
                    .InColumnValue("counter", exchange ? counter + row.Counter : row.Counter)
                    .InColumnValue("tag", row.Tag)
                    .InColumnValue("lastmodified", DateTime.UtcNow));
            });
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var q = new SqlQuery("tenants_quotarow").Select("path", "datetime", "counter", "tag", "tenant", "name");
            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("tenant", tenant);
            }
            if (name != null)
            {
                q.Where("name", name);
            }
            if (query != TenantQuotaRowQuery.All)
            {
                if (!string.IsNullOrEmpty(query.Path)) q.Where("path", query.Path);
                if (query.LastModified != default(DateTime)) q.Where("lastmodified", query.LastModified);
            }

            return ExecList(q)
                .ConvertAll(r => new TenantQuotaRow((string)r[0], Convert.ToInt64(r[2]), (string)r[3])
                {
                    DateTime = (DateTime)r[1],
                    Tenant = Convert.ToInt32(r[4]),
                    Name = (string)r[5]
                });
        }
    }
}
