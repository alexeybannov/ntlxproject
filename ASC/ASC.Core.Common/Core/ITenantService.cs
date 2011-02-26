using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface ITenantService
    {
        void ValidateDomain(string domain);

        IEnumerable<Tenant> GetTenants(DateTime from);

        IEnumerable<Tenant> GetTenants(string login, string password);

        Tenant GetTenant(int id);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int id);


        byte[] GetTenantSettings(int tenant, string key);

        void SetTenantSettings(int tenant, string key, byte[] data);
    }
}
