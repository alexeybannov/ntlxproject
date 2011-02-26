using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using ASC.Core.Tenants;


namespace ASC.Core
{
    class ClientTenantManager : ITenantManagerClient
    {
        private readonly ITenantService tenantService;
        private readonly IQuotaService quotaService;
        private const string CURRENT_TENANT = "CURRENT_TENANT";


        public ClientTenantManager(ITenantService tenantService, IQuotaService quotaService)
        {
            this.tenantService = tenantService;
            this.quotaService = quotaService;
        }


        public List<Tenant> GetTenants()
        {
            return tenantService.GetTenants(default(DateTime)).ToList();
        }

        public Tenant GetTenant(int tenantId)
        {
            return GetTenants().SingleOrDefault(t => t.TenantId == tenantId);
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");

            var tenants = GetTenants();
            var tenant = tenants.Find(t => t.MappedDomains.Contains(domain.ToLowerInvariant()));
            if (tenant == null && CoreContext.Configuration.Standalone)
            {
                tenant = tenants.Find(t => t.TenantId == 0);
                if (tenant == null) return tenants.FirstOrDefault();
            }
            return tenant;
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            return tenantService.SaveTenant(tenant);
        }

        public void RemoveTenant(int tenantId)
        {
            tenantService.RemoveTenant(tenantId);
        }

        public Tenant GetCurrentTenant()
        {
            return GetCurrentTenant(true);
        }

        public Tenant GetCurrentTenant(bool throwOnError)
        {
            var tenant = CallContext.GetData(CURRENT_TENANT) as Tenant;
            if (tenant == null && HttpContext.Current != null)
            {
                tenant = HttpContext.Current.Items[CURRENT_TENANT] as Tenant;
                if (tenant == null && HttpContext.Current.Request != null)
                {
                    tenant = GetTenant(HttpContext.Current.Request.Url.Host);
                    HttpContext.Current.Items[CURRENT_TENANT] = tenant;
                }
            }
            if (tenant == null && throwOnError)
            {
                throw new Exception("Could not resolve current tenant :-(.");
            }
            return tenant;
        }

        public void SetCurrentTenant(Tenant tenant)
        {
            if (tenant != null)
            {
                CallContext.SetData(CURRENT_TENANT, tenant);
                Thread.CurrentThread.CurrentCulture = tenant.GetCulture();
                Thread.CurrentThread.CurrentUICulture = tenant.GetCulture();
            }
        }

        public void SetCurrentTenant(int tenantId)
        {
            SetCurrentTenant(GetTenant(tenantId));
        }

        public void SetCurrentTenant(string domain)
        {
            SetCurrentTenant(GetTenant(domain));
        }

        public void CheckTenantAddress(string address)
        {
            tenantService.ValidateDomain(address);
        }

        
        public TenantQuota GetTenantQuota(int tenant, string name)
        {
            return quotaService.GetTenantQuota(tenant, name);
        }

        public void SetTenantQuota(int tenant, string name, TenantQuota quota)
        {
            quotaService.SetTenantQuota(tenant, name, quota);
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            quotaService.SetTenantQuotaRow(tenant, name, row, exchange);
        }

        public List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            return quotaService.FindTenantQuotaRows(tenant, name, query).ToList();
        }
    }
}
