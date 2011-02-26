using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using ASC.Core.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

namespace ASC.Core
{
    public class HostedSolution
    {
        private readonly ITenantService tenantService;
        private readonly IUserService userService;


        public HostedSolution(ConnectionStringSettings connectionString)
        {
            tenantService = new DbTenantService(connectionString);
            userService = new DbUserService(connectionString);
        }

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }

        public List<Tenant> FindTenants(string login, string password)
        {
            if (password != null && userService.GetUser(Tenant.DEFAULT_TENANT, login, Hasher.Base64Hash(password, HashAlg.SHA256)) == null)
            {
                throw new SecurityException("Invalid login or password.");
            }
            return tenantService.GetTenants(login, Hasher.Base64Hash(password, HashAlg.SHA256)).ToList();
        }

        public void CheckTenantAddress(string address)
        {
            tenantService.ValidateDomain(address);
        }

        public string RegisterTenant(TenantRegistrationInfo ri)
        {
            ValidateTenantRegistrationInfo(ri);

            var tenant = new Tenant(ri.Address.ToLowerInvariant())
            {
                Name = ri.Name,
                Language = ri.Culture.Name,
                TimeZone = ri.TimeZoneInfo,
                OwnerName = string.Format("{0} {1}", ri.FirstName, ri.LastName),
                OwnerEMail = ri.Email
            };
            tenant = tenantService.SaveTenant(tenant);

            var user = new User()
            {
                UserName = ri.Email.Substring(0, ri.Email.IndexOf('@')),
                LastName = ri.LastName,
                FirstName = ri.FirstName,
                Email = ri.Email,
            };
            user = userService.SaveUser(tenant.TenantId, user);
            userService.SetUserPassword(tenant.TenantId, user.Id, ri.Password);
            userService.SaveUserGroupRef(tenant.TenantId, new UserGroupRef(user.Id, Constants.GroupAdmin.ID, UserGroupRefType.Contains));

            return SecurityContext.AuthenticateMe(ri.Email, ri.Password);
        }

        public string CreateAuthenticationCookie(int tenantID, string login, string password)
        {
            var credential = AuthenticationContext.CreateCredential(tenantID, login, password);
            return CookieStorage.Save(credential);
        }


        private void ValidateTenantRegistrationInfo(TenantRegistrationInfo ri)
        {
            if (ri == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(ri.Name)) throw new Exception("Community name can not be empty");
            if (string.IsNullOrEmpty(ri.Address)) throw new Exception("Community address can not be empty");

            if (string.IsNullOrEmpty(ri.Email)) throw new Exception("Account email can not be empty");
            if (string.IsNullOrEmpty(ri.FirstName)) throw new Exception("Account firstname can not be empty");
            if (string.IsNullOrEmpty(ri.LastName)) throw new Exception("Account lastname can not be empty");
            if (string.IsNullOrEmpty(ri.Password)) ri.Password = Crypto.GeneratePassword(6);
        }
    }
}