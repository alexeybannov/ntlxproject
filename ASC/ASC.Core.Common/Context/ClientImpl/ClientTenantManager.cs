#region usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using ASC.Common.Services;
using ASC.Core.Common.Cache;
using ASC.Core.Configuration;
using ASC.Core.Tenants;
using Constants = ASC.Core.Users.Constants;

#endregion

namespace ASC.Core
{
    class ClientTenantManager : ITenantManagerClient
    {
        private const string CURRENT_TENANT = "CURRENT_TENANT";
        private readonly ICache<int, Tenant> cache;
        private readonly Dictionary<string, bool> hostCache = new Dictionary<string, bool>();

        internal ClientTenantManager()
        {
            try
            {
                TenantThrow(false);
                cache = CoreContext.CacheInfoStorage.CreateCache<int, Tenant>(Constants.CacheIdTenants, SyncTenants);
            }
            finally
            {
                TenantThrow(true);
            }
        }

        #region ITenantManager Members

        public List<Tenant> GetTenants()
        {
            try
            {
                FlushCacheIfNeed();
                TenantThrow(false);
                return cache.Values
                    .OrderBy(t => t.TenantId)
                    .ToList();
            }
            finally
            {
                TenantThrow(true);
            }
        }

        public Tenant GetTenant(int tenantId)
        {
            try
            {
                FlushCacheIfNeed();
                TenantThrow(false);
                return cache.ContainsKey(tenantId) ? cache[tenantId] : null;
            }
            finally
            {
                TenantThrow(true);
            }
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");
            try
            {
                TenantThrow(false);
                Tenant tenant = GetTenants().Find(t => t.MappedDomains.Contains(domain.ToLowerInvariant()));
                if (tenant == null && CoreContext.Configuration.Standalone)
                {
                    tenant = GetTenants().Find(t => t.TenantId == 0);
                    if (tenant == null) return GetTenants().FirstOrDefault();
                }
                return tenant;
            }
            finally
            {
                TenantThrow(true);
            }
        }

        public Tenant SaveTenant(Tenant tenant)
        {
            try
            {
                TenantThrow(false);
                tenant = CoreContext.InternalTenantManager.SaveTenant(tenant);
                cache[tenant.TenantId] = tenant;
                return tenant;
            }
            finally
            {
                TenantThrow(true);
            }
        }

        public void RemoveTenant(int tenantId)
        {
            try
            {
                TenantThrow(false);
                CoreContext.InternalTenantManager.RemoveTenant(tenantId);
                cache.Remove(tenantId);
            }
            finally
            {
                TenantThrow(true);
            }
        }

        public Tenant GetCurrentTenant()
        {
            return GetCurrentTenant(true);
        }

        public Tenant GetCurrentTenant(bool throwOnError)
        {
            var data = CallContext.GetData(CURRENT_TENANT) as TenantLogicalContextData;
            Tenant tenant = data != null ? data.Tenant : null;
            if (tenant != null) return tenant;
            if (HttpContext.Current != null)
            {
                tenant = HttpContext.Current.Items[CURRENT_TENANT] as Tenant;
                if (tenant == null)
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
            if (tenant == null) return;
            CallContext.SetData(CURRENT_TENANT, new TenantLogicalContextData(tenant));
            CultureInfo culture = tenant.GetCulture();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public void SetCurrentTenant(int tenantId)
        {
            SetCurrentTenant(GetTenant(tenantId));
        }

        public void SetCurrentTenant(string domain)
        {
            SetCurrentTenant(GetTenant(domain));
        }

        public TenantQuota GetTenantQuota(int tenant, string name)
        {
            return CoreContext.InternalTenantManager.GetTenantQuota(tenant, name);
        }

        public void SetTenantQuota(int tenant, string name, TenantQuota quota)
        {
            CoreContext.InternalTenantManager.SetTenantQuota(tenant, name, quota);
        }

        public void SetTenantQuotaRow(int tenant, string name, TenantQuotaRow row, bool exchange)
        {
            CoreContext.InternalTenantManager.SetTenantQuotaRow(tenant, name, row, exchange);
        }

        public List<TenantQuotaRow> FindTenantQuotaRows(int tenant, string name, TenantQuotaRowQuery query)
        {
            return CoreContext.InternalTenantManager.FindTenantQuotaRows(tenant, name, query);
        }

        public void CheckTenantAddress(string address)
        {
            CoreContext.InternalTenantManager.CheckTenantAddress(address);
        }

        #endregion

        internal static void TenantThrow()
        {
            if (GetTenantThrowCount() == 0)
            {
                Tenant tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                if (tenant != null) CoreContext.TenantManager.SetCurrentTenant(tenant);
            }
        }

        private void TenantThrow(bool tenantThrow)
        {
            SetTenantThrowCount(GetTenantThrowCount() + (tenantThrow ? 1 : -1));
        }

        private static int GetTenantThrowCount()
        {
            object count = CallContext.GetData("TENANT_THROW_COUNT");
            return count != null ? (int)count : 0;
        }

        private static void SetTenantThrowCount(int count)
        {
            CallContext.SetData("TENANT_THROW_COUNT", count);
        }

        private IDictionary<int, Tenant> SyncTenants()
        {
            return CoreContext.InternalTenantManager.GetTenants().ToDictionary(t => t.TenantId);
        }

        private void FlushCacheIfNeed()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                string host = HttpContext.Current.Request.Url.Host;
                if (string.IsNullOrEmpty(host) || hostCache.ContainsKey(host)) return;
                lock (hostCache)
                {
                    hostCache[host] = true;
                    CoreContext.CacheInfoStorage.ResetTrust(null);
                }
            }
        }

        [Serializable]
        private class TenantLogicalContextData : ILogicalThreadAffinative
        {
            public Tenant Tenant { get; private set; }

            public TenantLogicalContextData(Tenant tenant)
            {
                Tenant = tenant;
            }
        }
    }
}