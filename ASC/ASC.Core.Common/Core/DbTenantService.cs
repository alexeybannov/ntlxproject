using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using ASC.Security.Cryptography;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public class DbTenantService : DbBaseService, ITenantService
    {
        public DbTenantService(ConnectionStringSettings connectionString)
            : base(connectionString, null)
        {

        }


        public List<Tenant> GetTenants(DateTime from)
        {
            throw new NotImplementedException();
        }

        public Tenant GetTenant(int id)
        {
            throw new NotImplementedException();
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            throw new NotImplementedException();
        }

        public void RemoveTenant(int id)
        {
            var d = new SqlDelete("tenants_tenants").Where("id", id);
            ExecNonQuery(d);
        }
    }
}
