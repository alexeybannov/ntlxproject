using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core.Configuration.DAO
{
	public interface ITenantDAO
	{
		List<Tenant> GetTenants();

		List<Tenant> GetTenants(string login, string password);

		void CheckTenantAddress(string address);

		Tenant GetTenant(int tenantId);

		Tenant SaveTenant(Tenant tenant);

		void RemoveTenant(int tenantId);


		TenantQuota GetTenantQuota(int tenant, string name);

		void SetTenantQuota(int tenant, string name, TenantQuota quota);

		void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange);

		List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query);
	}
}