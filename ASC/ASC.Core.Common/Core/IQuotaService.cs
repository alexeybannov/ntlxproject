using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface IQuotaService
    {
        TenantQuota GetTenantQuota(int tenant, string name);

        void SetTenantQuota(int tenant, string name, TenantQuota quota);

        void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange);

        IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query);
    }
}
