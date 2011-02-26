using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Core.Users.DAO
{
    class AzDAO : DAOBase
    {
        public AzDAO(string dbId, int tenant)
            : base(dbId, tenant)
        {

        }

        #region IAzDao

        public List<AzRecord> GetAce(Guid? subjectID, Guid? actionID, string objectId)
        {
            //rows with tenant = -1 - common,
            //if exists same row but tenant != -1 - row deleted
            //if acetype not equal then row with tenant = -1 ignore
            var query = Query("core_acl").Select("Ace", "Creator");
            var where = Exp.Empty;
            if (subjectID.HasValue) where = where & Exp.Eq("Subject", subjectID.Value.ToString());
            if (actionID.HasValue) where = where & Exp.Eq("Action", actionID.Value.ToString());
            if (!string.IsNullOrEmpty(objectId)) where = where & Exp.Eq("Object", objectId);

            var aces = DbManager
                .ExecuteList(query.Where(where))
                .ConvertAll<AzRecord>(r => ToRecord(r));

            query = new SqlQuery("core_acl").Select("Ace", "Creator").Where("Tenant", -1);
            DbManager
                .ExecuteList(query.Where(where))
                .ConvertAll<AzRecord>(r => ToRecord(r))
                .ForEach(aceBase =>
                {
                    if (aces.RemoveAll(ace => ace.Id == aceBase.Id) == 0)
                    {
                        if (!aces.Exists(ace => ace.SubjectId == aceBase.SubjectId && ace.ActionId == aceBase.ActionId && ace.FullObjectId == aceBase.FullObjectId))
                        {
                            aces.Add(aceBase);
                        }
                    }
                });
            return aces;
        }

        public void SaveAce(AzRecord r)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                var count = DbManager.ExecuteScalar<int>(new SqlQuery("core_acl").SelectCount().Where("ace", r.Id).Where("tenant", -1));
                if (count == 0)
                {
                    DbManager.ExecuteNonQuery(
                        Insert("core_acl")
                        .InColumns("Ace", "Subject", "Action", "Object", "AceType", "Creator")
                        .Values(r.Id, r.SubjectId.ToString(), r.ActionId.ToString(), r.FullObjectId, r.Reaction, r.Creator)
                    );
                }
                else
                {
                    DbManager.ExecuteNonQuery(Delete("core_acl").Where("Ace", r.Id));
                }
                tx.Commit();
            }
        }

        public void RemoveAce(AzRecord r)
        {
            using (var tx = DbManager.BeginTransaction())
            {
                DbManager.ExecuteNonQuery(Delete("core_acl").Where("Ace", r.Id));
                var count = DbManager.ExecuteScalar<int>(new SqlQuery("core_acl").SelectCount().Where("ace", r.Id).Where("tenant", -1));
                if (count != 0)
                {
                    DbManager.ExecuteNonQuery(
                        Insert("core_acl")
                        .InColumns("Ace", "Subject", "Action", "Object", "AceType", "Creator")
                        .Values(r.Id, r.SubjectId.ToString(), r.ActionId.ToString(), r.FullObjectId, r.Reaction, r.Creator)
                    );
                }
                tx.Commit();
            }
        }

        /// <inheritdoc/>
        public List<AzObjectInfo> GetAzObjectInfos()
        {
            return new List<AzObjectInfo>();
        }

        /// <inheritdoc/>
        public AzObjectInfo GetAzObjectInfo(string objectId)
        {
            return null;
        }

        /// <inheritdoc/>
        public void SaveAzObjectInfo(AzObjectInfo azObjectInfo)
        {

        }

        /// <inheritdoc/>
        public void RemoveAzObjectInfo(AzObjectInfo azObjectInfo)
        {

        }

        #endregion

        private AzRecord ToRecord(object[] r)
        {
            var record = AzRecord.Parse(Convert.ToString(r[0]));
            record.Creator = (string)r[1];
            return record;
        }
    }
}