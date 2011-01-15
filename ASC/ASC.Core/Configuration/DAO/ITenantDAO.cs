using System;
using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core.Configuration.DAO
{
	public interface ITenantDAO
	{
		List<Tenant> GetTenants();

		List<Tenant> FindTenants(string login, string password);

		void CheckTenantAddress(string address);

		Tenant GetTenant(int tenantId);

		Tenant GetTenant(string domain);

		Tenant SaveTenant(Tenant tenant);

		void RemoveTenant(int tenantId);


		string SaveTenantInterim(TenantRegistrationInfo registrationInfo);

		TenantRegistrationInfo GetTenantInterim(string tenantInterimKey);

		void InitializeTemplateData(int tenant, Guid user, string[] sqlInstructions);


        List<TenantOwner> GetTenantOwners();
        
		TenantOwner GetTenantOwner(Guid ownerId);

		void SaveTenantOwner(TenantOwner owner);


		TenantQuota GetTenantQuota(int tenant, string name);

		void SetTenantQuota(int tenant, string name, TenantQuota quota);

		void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange);

		List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query);
	}
}