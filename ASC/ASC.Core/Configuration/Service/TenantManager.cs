using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration.DAO;
using ASC.Core.Configuration.Service;
using ASC.Core.Factories;
using ASC.Core.Tenants;
using ASC.Common.Security.Authorizing;

[assembly: AssemblyServices(typeof(TenantManager))]

namespace ASC.Core.Configuration.Service
{
    class TenantManager : RemotingServiceController, ITenantManager
    {
        private IDAOFactory factory;


        public TenantManager(IDAOFactory factory)
            : base(Constants.TenantManagerServiceInfo)
        {
            this.factory = factory;
        }

        #region ITenantManager Members

        public List<Tenant> GetTenants()
        {
            return GetTenantDAO().GetTenants();
        }

        public Tenant GetTenant(int tenantId)
        {
            return GetTenantDAO().GetTenant(tenantId);
        }

        public Tenant GetTenant(string domain)
        {
            return GetTenantDAO().GetTenant(domain);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public Tenant SaveTenant(Tenant tenant)
        {
            return GetTenantDAO().SaveTenant(tenant);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void RemoveTenant(int tenantId)
        {
            GetTenantDAO().RemoveTenant(tenantId);
        }

        public Tenant GetCurrentTenant()
        {
            throw new NotImplementedException();
        }

        public Tenant GetCurrentTenant(bool throwOnError)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentTenant(Tenant tenant)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentTenant(int tenantId)
        {
            throw new NotImplementedException();
        }

        public void SetCurrentTenant(string domain)
        {
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Role.Administrators)]
        [PrincipalPermission(SecurityAction.Demand, Role = Role.System)]
        public TenantOwner GetTenantOwner(Guid ownerId)
        {
            return GetTenantDAO().GetTenantOwner(ownerId);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Role.Administrators)]
        [PrincipalPermission(SecurityAction.Demand, Role = Role.System)]
        public void SaveTenantOwner(TenantOwner owner)
        {
            GetTenantDAO().SaveTenantOwner(owner);
        }


        public TenantQuota GetTenantQuota(int tenant, string name)
        {
            return GetTenantDAO().GetTenantQuota(tenant, name);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Role.Administrators)]
        [PrincipalPermission(SecurityAction.Demand, Role = Role.System)]
        public void SetTenantQuota(int tenant, string name, TenantQuota quota)
        {
            GetTenantDAO().SetTenantQuota(tenant, name, quota);
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            GetTenantDAO().SetTenantQuotaRow(tenant, name, row, exchange);
        }

        public List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            return GetTenantDAO().FindTenantQuotaRows(tenant, name, query);
        }

        public void CheckTenantAddress(string address)
        {
            GetTenantDAO().CheckTenantAddress(address);
        }

        #endregion

        private ITenantDAO GetTenantDAO()
        {
            return factory.GetTenantDAO();
        }
    }
}