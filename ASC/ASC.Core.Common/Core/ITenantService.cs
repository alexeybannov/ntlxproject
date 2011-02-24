using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface ITenantService
    {
        IEnumerable<Tenant> GetTenants(DateTime from);

        Tenant GetTenant(int id);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int id);
    }
}
