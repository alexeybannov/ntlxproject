using System.Web;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using System;

namespace ASC.Core.Users.DAO
{
    abstract class DAOBase : IDisposable
    {
        private string dbId;

        private DbManager dbManager;

        protected int Tenant
        {
            get;
            private set;
        }


        protected DAOBase(string dbId)
            : this(dbId, -1)
        {
        }

        protected DAOBase(string dbId, int tenant)
        {
            this.dbId = dbId;
            this.Tenant = tenant;
        }

        protected DbManager DbManager
        {
            get
            {
                if (dbManager == null)
                {
                    dbManager = HttpContext.Current != null ? DbManager.FromHttpContext(dbId) : new DbManager(dbId);
                }
                return dbManager;
            }
        }

        protected SqlQuery Query(string table)
        {
            var query = new SqlQuery(table);
            if (Tenant != -1) query.Where(GetTenantColumnName(table), Tenant);
            return query;
        }

        protected SqlDelete Delete(string table)
        {
            var delete = new SqlDelete(table);
            if (Tenant != -1) delete.Where(GetTenantColumnName(table), Tenant);
            return delete;
        }

        protected SqlInsert Insert(string table)
        {
            var insert = new SqlInsert(table, true);
            if (Tenant != -1) insert.InColumnValue(GetTenantColumnName(table), Tenant);
            return insert;
        }

        protected SqlUpdate Update(string table)
        {
            var update = new SqlUpdate(table);
            if (Tenant != -1) update.Where(GetTenantColumnName(table), Tenant);
            return update;
        }

        private string GetTenantColumnName(string table)
        {
            var tenant = "Tenant";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (HttpContext.Current == null && dbManager != null)
            {
                dbManager.Dispose();
                dbManager = null;
            }
        }

        #endregion
    }
}
