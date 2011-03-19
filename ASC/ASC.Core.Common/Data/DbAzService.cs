using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Security.Authorizing;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbAzService : DbBaseService, IAzService
    {
        public DbAzService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }


        public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
        {
            var q = new SqlQuery("core_acl")
                .Select("subject", "action", "object", "acetype", "tenant", "creator")
                .OrderBy("tenant", false);
            if (tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where(Exp.Eq("tenant", Tenant.DEFAULT_TENANT) | Exp.Eq("tenant", tenant));
            }

            return ExecList(q).Select(r => ToRecord(r));
        }

        public AzRecord SaveAce(int tenant, AzRecord r)
        {
            r.Tenant = tenant;
            var i = new SqlInsert("core_acl", true)
                .InColumnValue("subject", r.SubjectId.ToString())
                .InColumnValue("action", r.ActionId.ToString())
                .InColumnValue("object", r.ObjectId ?? string.Empty)
                .InColumnValue("acetype", r.Reaction)
                .InColumnValue("creator", r.Creator)
                .InColumnValue("tenant", r.Tenant);

            ExecNonQuery(i);
            return r;
        }

        public void RemoveAce(int tenant, AzRecord r)
        {
            r.Tenant = tenant;
            var d = new SqlDelete("core_acl")
                .Where("subject", r.SubjectId)
                .Where("action", r.ActionId)
                .Where("object", r.ObjectId ?? string.Empty)
                .Where("acetype", r.Reaction)
                .Where("tenant", r.Tenant);

            ExecNonQuery(d);
        }


        private AzRecord ToRecord(object[] r)
        {
            return new AzRecord(
                new Guid((string)r[0]),
                new Guid((string)r[1]),
                (AceType)Convert.ToInt32(r[2]),
                string.Empty.Equals(r[3]) ? null : (string)r[3])
                {
                    Tenant = Convert.ToInt32(r[4]),
                    Creator = (string)r[5],
                };
        }
    }
}