using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public class DbTenantService : DbBaseService, ITenantService
    {
        private List<string> forbiddenDomains;

        private Regex validDomain = new Regex("^[a-z0-9]([a-z0-9-]){1,98}[a-z0-9]$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);


        public DbTenantService(ConnectionStringSettings connectionString)
            : base(connectionString, null)
        {
            forbiddenDomains = ExecList(new SqlQuery("tenants_forbiden").Select("address"))
                .Select(r => (string)r[0])
                .ToList();
        }


        public IEnumerable<Tenant> GetTenants(DateTime from)
        {
            var q = GetTenantQuery(from != default(DateTime) ? Exp.Ge("last_modified", from) : Exp.Empty);
            return ExecList(q)
                .Select(r => ToTenant(r));
        }

        public Tenant GetTenant(int id)
        {
            var q = GetTenantQuery(Exp.Eq("id", id));
            return ExecList(q)
                .Select(r => ToTenant(r))
                .SingleOrDefault();
        }

        public Tenant SaveTenant(Tenant t)
        {
            if (t == null) throw new ArgumentNullException("tenant");

            ExecAction(db =>
            {
                var isnew = t.TenantId == Tenant.DEFAULT_TENANT;

                ValidateDomain(db, t.TenantAlias, t.TenantId);
                if (!string.IsNullOrEmpty(t.MappedDomain)) ValidateDomain(db, t.MappedDomain, t.TenantId);

                var i = new SqlInsert("tenants_tenants")
                    .InColumnValue("id", t.TenantId)
                    .InColumnValue("alias", t.TenantAlias)
                    .InColumnValue("mappeddomain", t.MappedDomain)
                    .InColumnValue("name", t.Name ?? t.TenantAlias)
                    .InColumnValue("language", t.Language)
                    .InColumnValue("timezone", t.TimeZone.Id)
                    .InColumnValue("owner_name", t.OwnerName)
                    .InColumnValue("owner_email", t.OwnerEMail)
                    .InColumnValue("trusteddomains", t.GetTrustedDomains())
                    .InColumnValue("trusteddomainsenabled", t.TrustedDomainsEnabled)
                    .InColumnValue("creationdatetime", t.CreatedDateTime)
                    .InColumnValue("status", (int)t.Status)
                    .InColumnValue("statuschanged", t.StatusChangeDate)
                    .Identity<int>(0, -1, true);

                t.TenantId = db.ExecScalar<int>(i);

                if (isnew)
                {
                    db.ExecNonQuery(
                        new SqlInsert("core_subscription")
                        .InColumns("source", "action", "recipient", "object", "unsubscribed", "tenant")
                        .Values(new SqlQuery("tenants_template_subscription").Select("source", "action", "recipient", "object", "unsubscribed", t.TenantId.ToString()))
                    );
                    db.ExecNonQuery(
                        new SqlInsert("core_subscriptionmethod")
                        .InColumns("source", "action", "recipient", "sender", "tenant")
                        .Values(new SqlQuery("tenants_template_subscriptionmethod").Select("source", "action", "recipient", "sender", t.TenantId.ToString()))
                    );
                }
            });
            CalculateTenantDomain(t);
            return t;
        }

        public void RemoveTenant(int id)
        {
            var d = new SqlDelete("tenants_tenants").Where("id", id);
            ExecNonQuery(d);
        }


        private SqlQuery GetTenantQuery(Exp where)
        {
            return new SqlQuery("tenants_tenants t")
                .Select("id", "alias", "mappeddomain", "name", "language", "timezone", "owner_name", "owner_email")
                .Select("trusteddomains", "trusteddomainsenabled", "creationdatetime", "status", "statuschanged")
                .Where(where);
        }

        private Tenant ToTenant(object[] r)
        {
            var tenant = new Tenant(Convert.ToInt32(r[0]), (string)r[1])
            {
                MappedDomain = (string)r[2],
                Name = (string)r[3],
                Language = (string)r[4],
                TimeZone = GetTimeZone((string)r[5]),
                OwnerName = (string)r[6],
                OwnerEMail = (string)r[7],
                TrustedDomainsEnabled = Convert.ToBoolean(r[9]),
                CreatedDateTime = (DateTime)r[10],
                Status = (TenantStatus)Convert.ToInt32(r[11]),
                StatusChangeDate = (DateTime)r[12],
            };
            tenant.SetTrustedDomains((string)r[8]);
            CalculateTenantDomain(tenant);

            return tenant;
        }

        private void CalculateTenantDomain(Tenant tenant)
        {
            tenant.MappedDomains.Clear();
            var baseHost = ConfigurationManager.AppSettings["asc.core.tenants.base-domain"];
            if (baseHost == "localhost")
            {
                //single tenant on local host
                tenant.TenantDomain = Dns.GetHostName();
                tenant.TenantAlias = "localhost";
                tenant.MappedDomains.Add("localhost");
            }
            else
            {
                tenant.TenantDomain = tenant.TenantAlias;
                if (!string.IsNullOrEmpty(baseHost) && tenant.TenantAlias != baseHost)
                {
                    tenant.TenantDomain += "." + baseHost;
                }
                if (tenant.TenantId == 0)
                {
                    tenant.MappedDomains.Add(Dns.GetHostName().ToLowerInvariant());
                    tenant.MappedDomains.Add("localhost");
                }
            }
            tenant.MappedDomains.Add(tenant.TenantDomain.ToLowerInvariant());
            if (!string.IsNullOrEmpty(tenant.MappedDomain))
            {
                tenant.TenantDomain = tenant.MappedDomain;
                tenant.MappedDomains.Add(tenant.MappedDomain.ToLowerInvariant());
            }
            tenant.TenantDomain = tenant.TenantDomain.ToLowerInvariant();
        }

        private TimeZoneInfo GetTimeZone(string zoneId)
        {
            if (!string.IsNullOrEmpty(zoneId))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(zoneId);
                }
                catch (TimeZoneNotFoundException) { }
            }
            return TimeZoneInfo.Utc;
        }

        private void ValidateDomain(IDbExecuter db, string domain, int tenantId)
        {
            if (string.IsNullOrEmpty(domain))
            {
                throw new TenantTooShortException("Tenant domain can not be empty.");
            }
            if (domain.Length < 6 || 100 <= domain.Length)
            {
                throw new TenantTooShortException("Length of domain must be greater than or equal to 6 and less than or equal to 100.");
            }
            if (!validDomain.IsMatch(domain))
            {
                throw new TenantIncorrectCharsException("Domain contains invalid characters.");
            }
            if (tenantId != 0 && forbiddenDomains.Contains(domain, StringComparer.InvariantCultureIgnoreCase))
            {
                throw new TenantAlreadyExistsException(domain);
            }

            var count = db.ExecScalar<int>(
                new SqlQuery("tenants_tenants")
                .SelectCount()
                .Where(Exp.Eq("alias", domain.ToLowerInvariant()) | Exp.Eq("mappeddomain", domain.ToLowerInvariant()))
                .Where(!Exp.Eq("id", tenantId)));
            if (count != 0)
            {
                throw new TenantAlreadyExistsException(domain);
            }
        }
    }
}
