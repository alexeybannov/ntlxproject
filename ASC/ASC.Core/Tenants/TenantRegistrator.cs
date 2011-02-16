using System;
using ASC.Core.Configuration.DAO;
using ASC.Core.Users;

namespace ASC.Core.Tenants
{
    public class TenantRegistrator
    {
        private ITenantDAO dao;


        public TenantRegistrator(ITenantDAO dao)
        {
            this.dao = dao;
        }

        public void CheckTenantAddress(string address)
        {
            dao.CheckTenantAddress(address);
        }

        public string RegisterTenant(TenantRegistrationInfo registrationInfo)
        {
            ValidateTenantRegistrationInfo(registrationInfo);

            bool doLogout = true;
            try
            {
                SecurityContext.AuthenticateMe(Configuration.Constants.CoreSystem);

                var owner = new TenantOwner(registrationInfo.Email)
                {
                    FirstName = registrationInfo.FirstName,
                    LastName = registrationInfo.LastName,
                };
                CoreContext.Authentication.SetUserPassword(owner.Id, registrationInfo.Password);
                CoreContext.TenantManager.SaveTenantOwner(owner);

                var tenant = new Tenant(registrationInfo.Address.ToLowerInvariant())
                {
                    Name = registrationInfo.Name,
                    Language = registrationInfo.Culture.Name,
                    TimeZone = registrationInfo.TimeZoneInfo,
                    OwnerId = owner.Id,
                };
                tenant.TrustedDomains.AddRange(registrationInfo.TrustedDomains);
                tenant = CoreContext.TenantManager.SaveTenant(tenant);
                CoreContext.TenantManager.SetCurrentTenant(tenant);

                var user = new UserInfo()
                {
                    ID = owner.Id,
                    LastName = registrationInfo.LastName,
                    FirstName = registrationInfo.FirstName,
                    Email = registrationInfo.Email,
                    UserName = registrationInfo.Email.Substring(0, registrationInfo.Email.IndexOf('@'))
                };
                user = CoreContext.UserManager.SaveUserInfo(user);
                CoreContext.Authentication.SetUserPassword(user.ID, registrationInfo.Password);
                CoreContext.UserManager.AddUserIntoGroup(user.ID, Constants.GroupAdmin.ID);

                SecurityContext.Logout();
                doLogout = false;
                return SecurityContext.AuthenticateMe(registrationInfo.Email, registrationInfo.Password);
            }
            finally
            {
                if (doLogout) SecurityContext.Logout();
            }
        }


        private void ValidateTenantRegistrationInfo(TenantRegistrationInfo registrationInfo)
        {
            if (registrationInfo == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(registrationInfo.Name)) throw new Exception("Community name can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.Address)) throw new Exception("Community address can not be empty");

            if (string.IsNullOrEmpty(registrationInfo.Email)) throw new Exception("Account email can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.FirstName)) throw new Exception("Account firstname can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.LastName)) throw new Exception("Account lastname can not be empty");
            if (registrationInfo.Password == null) registrationInfo.Password = Crypto.GeneratePassword(6);
        }        
    }
}
