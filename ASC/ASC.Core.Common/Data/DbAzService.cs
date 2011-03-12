using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Security.Authorizing;
using ASC.Core.Data;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbAzService : DbBaseService, IDbAzService
    {
        public DbAzService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }


        public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
        {
            //rows with tenant = -1 - common,
            //if exists same row but tenant != -1 - row deleted
            //if acetype not equal then row with tenant = -1 ignore
            var q = new SqlQuery("core_acl")
                .Select("subject", "action", "object", "acetype", "tenant", "creator")
                .Where(Exp.Eq("tenant", Tenant.DEFAULT_TENANT) | Exp.Eq("tenant", tenant))
                .OrderBy("tenant", true);

            var aces = ExecList(q).Select(r => ToRecord(r));
            var result = new List<AzRecord>();
            foreach (var ace in aces)
            {
                var exist = result.Find(a => a.SubjectId == ace.SubjectId && a.ActionId == ace.ActionId && a.ObjectId == ace.ObjectId);
                result.Remove(exist);
                if (exist.Reaction == ace.Reaction) result.Add(ace);
            }
            return result;
        }

        public void SaveAce(int tenant, AzRecord r)
        {
            ExecAction(db =>
            {
                var q = new SqlQuery("core_acl")
                    .SelectCount()
                    .Where("subject", r.SubjectId)
                    .Where("action", r.ActionId)
                    .Where("object", r.ObjectId)
                    .Where("acetype", r.Reaction)
                    .Where("tenant", -1);

                var count = db.ExecScalar<int>(q);
                ISqlInstruction i = null;
                if (count == 0)
                {
                    i = new SqlInsert("core_acl", true)
                        .InColumnValue("subject", r.SubjectId.ToString())
                        .InColumnValue("action", r.ActionId.ToString())
                        .InColumnValue("object", r.ObjectId)
                        .InColumnValue("acetype", r.Reaction)
                        .InColumnValue("creator", r.Creator)
                        .InColumnValue("tenant", tenant);
                }
                else
                {
                    i = new SqlDelete("core_acl")
                        .Where("subject", r.SubjectId)
                        .Where("action", r.ActionId)
                        .Where("object", r.ObjectId)
                        .Where("acetype", r.Reaction)
                        .Where("tenant", tenant);
                }
                db.ExecNonQuery(i);
            });
        }

        public void RemoveAce(int tenant, AzRecord r)
        {
            ExecAction(db =>
            {
                var d = new SqlDelete("core_acl")
                    .Where("subject", r.SubjectId)
                    .Where("action", r.ActionId)
                    .Where("object", r.ObjectId)
                    .Where("acetype", r.Reaction)
                    .Where("tenant", tenant);

                db.ExecNonQuery(d);
                
                var q = new SqlQuery("core_acl")
                    .SelectCount()
                    .Where("subject", r.SubjectId)
                    .Where("action", r.ActionId)
                    .Where("object", r.ObjectId)
                    .Where("acetype", r.Reaction)
                    .Where("tenant", -1);

                var count = db.ExecScalar<int>(q);
                if (count != 0)
                {
                    var i = new SqlInsert("core_acl", true)
                        .InColumnValue("subject", r.SubjectId.ToString())
                        .InColumnValue("action", r.ActionId.ToString())
                        .InColumnValue("object", r.ObjectId)
                        .InColumnValue("acetype", r.Reaction)
                        .InColumnValue("creator", r.Creator)
                        .InColumnValue("tenant", tenant);

                    db.ExecNonQuery(i);
                }
            });
        }


        private AzRecord ToRecord(object[] r)
        {
            return new AzRecord(
                new Guid((string)r[0]),
                new Guid((string)r[1]),
                (AceType)Convert.ToInt32(r[2]),
                string.Empty.Equals(r[3]) ? null : (string)r[3])
                {
                    Creator = (string)r[4],
                };
        }
    }
}