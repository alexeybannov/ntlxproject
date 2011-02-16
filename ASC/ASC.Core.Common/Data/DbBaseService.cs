using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public abstract class DbBaseService : IDbExecuter
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


        public T ExecScalar<T>(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteScalar<T>(sql);
            }
        }

        public int ExecNonQuery(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteNonQuery(sql);
            }
        }

        public List<object[]> ExecList(ISqlInstruction sql)
        {
            using (var db = new DbManager(dbid))
            {
                return db.ExecuteList(sql);
            }
        }

        public void ExecBatch(IEnumerable<ISqlInstruction> batch)
        {
            using (var db = new DbManager(dbid))
            {
                db.ExecuteBatch(batch);
            }
        }

        public void ExecAction(Action<IDbExecuter> action)
        {
            using (var db = new DbManager(dbid))
            using (var tx = db.BeginTransaction())
            {
                action(new DbExecuter(db));
                tx.Commit();
            }
        }


        private class DbExecuter : IDbExecuter
        {
            private DbManager db;

            
            public DbExecuter(DbManager db)
            {
                this.db = db;
            }


            public T ExecScalar<T>(ISqlInstruction sql)
            {
                return db.ExecuteScalar<T>(sql);
            }

            public int ExecNonQuery(ISqlInstruction sql)
            {
                return db.ExecuteNonQuery(sql);
            }

            public List<object[]> ExecList(ISqlInstruction sql)
            {
                return db.ExecuteList(sql);
            }

            public void ExecBatch(IEnumerable<ISqlInstruction> batch)
            {
                db.ExecuteBatch(batch);
            }

            public void ExecAction(Action<IDbExecuter> action)
            {
                using (var tx = db.BeginTransaction())
                {
                    action(this);
                    tx.Commit();
                }
            }
        }
    }
}
