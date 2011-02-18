﻿using System;
using System.Collections.Generic;
using System.Configuration;
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public abstract class DbBaseService
    {
        private readonly IDbExecuter db;

        protected string TenantColumn
        {
            get;
            private set;
        }


        protected DbBaseService(ConnectionStringSettings connectionString, string tenantColumn)
        {
            db = new DbExecuter(connectionString);
            TenantColumn = tenantColumn;
        }


        protected T ExecScalar<T>(ISqlInstruction sql)
        {
            return db.ExecScalar<T>(sql);
        }

        protected int ExecNonQuery(ISqlInstruction sql)
        {
            return db.ExecNonQuery(sql);
        }

        protected List<object[]> ExecList(ISqlInstruction sql)
        {
            return db.ExecList(sql);
        }

        protected void ExecBatch(IEnumerable<ISqlInstruction> batch)
        {
            db.ExecBatch(batch);
        }

        protected void ExecAction(Action<IDbExecuter> action)
        {
            db.ExecAction(action);
        }


        protected SqlQuery Query(string table, int tenant)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlInsert Insert(string table, int tenant)
        {
            return new SqlInsert(table, true).InColumnValue(GetTenantColumnName(table), tenant);
        }

        protected SqlUpdate Update(string table, int tenant)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), tenant);
        }

        protected SqlDelete Delete(string table, int tenant)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), tenant);
        }

        protected string GetTenantColumnName(string table)
        {
            var pos = table.LastIndexOf(' ');
            return (0 < pos ? table.Substring(pos).Trim() + '.' : string.Empty) + TenantColumn;
        }
    }
}