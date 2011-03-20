using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
        private readonly List<string> thisCompAddresses = new List<string>();


        public ClientTenantManager(ITenantService tenantService, IQuotaService quotaService)
        {
            this.tenantService = tenantService;
            this.quotaService = quotaService;

            thisCompAddresses.Add("localhost");
            thisCompAddresses.Add(Dns.GetHostName().ToLowerInvariant());
            thisCompAddresses.AddRange(Dns.GetHostAddresses("localhost").Select(a => a.ToString()));
            thisCompAddresses.AddRange(Dns.GetHostAddresses(Dns.GetHostName()).Select(a => a.ToString()));
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
            if (string.IsNullOrEmpty(domain)) return null;

            Tenant t = null;
            if (thisCompAddresses.Contains(domain, StringComparer.InvariantCultureIgnoreCase))
            {
                t = tenantService.GetTenant(0);
            }
            if (t == null)
            {
                var baseUrl = ConfigurationManager.AppSettings["asc.core.tenants.base-domain"];
                if (baseUrl != null && domain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    t = tenantService.GetTenant(domain.Substring(0, domain.Length - baseUrl.Length - 1));
                }
            }
            if (t == null)
            {
                t = tenantService.GetTenant(domain);
            }
            if (t == null && CoreContext.Configuration.Standalone)
            {
                t = GetTenants().FirstOrDefault();
            }
            return t;
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

        public void SetTenantQuota(TenantQuota quota)
        {
            quotaService.SetTenantQuota(quota);
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
