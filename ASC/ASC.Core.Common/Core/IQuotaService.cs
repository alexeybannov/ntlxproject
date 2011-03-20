using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface IQuotaService
    {
        IEnumerable<TenantQuota> GetTenantQuotas();

        TenantQuota GetTenantQuota(int tenant, string name);

        void SetTenantQuota(TenantQuota quota);

        
        void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange);

        IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query);
    }
}
