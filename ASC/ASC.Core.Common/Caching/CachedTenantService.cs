using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedTenantService : ITenantService
    {
        private ITenantService service;


        public CachedTenantService(ITenantService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
        }


        public void ValidateDomain(string domain)
        {
            service.ValidateDomain(domain);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from)
        {
            return service.GetTenants(from);
        }

        public IEnumerable<Tenant> GetTenants(string login, string password)
        {
            return service.GetTenants(login, password);
        }

        public Tenant GetTenant(int id)
        {
            throw new NotImplementedException();
        }

        public Tenant GetTenant(string domain)
        {
            throw new NotImplementedException();
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            throw new NotImplementedException();
        }

        public void RemoveTenant(int id)
        {
            throw new NotImplementedException();
        }

        public byte[] GetTenantSettings(int tenant, string key)
        {
            throw new NotImplementedException();
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
