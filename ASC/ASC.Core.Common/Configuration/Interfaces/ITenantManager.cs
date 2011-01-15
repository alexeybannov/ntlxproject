#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Services;
using ASC.Core.Tenants;

#endregion

namespace ASC.Core.Configuration
{
    [Service("{E0F0E200-563C-4CFF-89D4-F1AD623CCEFB}")]
    public interface ITenantManager : IService
    {
        List<Tenant> GetTenants();

        Tenant GetTenant(int tenantId);

        Tenant GetTenant(string domain);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int tenantId);

        Tenant GetCurrentTenant();

        Tenant GetCurrentTenant(bool throwOnError);

        void SetCurrentTenant(Tenant tenant);

        void SetCurrentTenant(int tenantId);

        void SetCurrentTenant(string domain);

        TenantOwner GetTenantOwner(Guid ownerId);

        void SaveTenantOwner(TenantOwner owner);

        TenantQuota GetTenantQuota(int tenant, string name);

        void SetTenantQuota(int tenant, string name, TenantQuota quota);

        void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange);

        List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query);

        void CheckTenantAddress(string address);
    }
}