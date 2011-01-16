using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public abstract class DbBaseService
    {
        private readonly string dbid;


        protected DbBaseService(ConnectionStringSettings connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");

            dbid = connectionString.Name;
            if (!DbRegistry.IsDatabaseRegistered(dbid))
            {
                DbRegistry.RegisterDatabase(dbid, connectionString);
            }
        }


        protected T ExecScalar<T>(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteScalar<T>(sql);
            }
        }

        protected int ExecNonQuery(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteNonQuery(sql);
            }
        }

        protected List<object[]> ExecList(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteList(sql);
            }
        }

        protected void ExecBatch(IEnumerable<ISqlInstruction> batch)
        {
            using (var db = new DbManager(dbid))
            {
                db.ExecuteBatch(batch);
            }
        }

        protected void ExecBatch(Action<DbManager> batch)
        {
            using (var db = new DbManager(dbid))
            using (var tx = db.BeginTransaction())
            {
                batch(db);
                tx.Commit();
            }
        }
    }
}
