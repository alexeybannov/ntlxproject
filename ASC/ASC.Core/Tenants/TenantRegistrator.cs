using System;
using System.Configuration;
using System.Data.Common;
using ASC.Common.Data;
using ASC.Core.Configuration.DAO;
using ASC.Core.Factories;
using ASC.Core.Users;

namespace ASC.Core.Tenants
{
    public class TenantRegistrator
    {
        private ITenantDAO dao;


        public TenantRegistrator(ConnectionStringSettings connectionString)
        {
            if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
            {
                DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, connectionString);
            }
            dao = new TenantDAO(DAOFactory.DAO_KEY);
        }

        public TenantRegistrator(DbProviderFactory dbProviderFactory, string connectionString)
        {
            if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
            {
                DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, dbProviderFactory, connectionString);
            }
            dao = new TenantDAO(DAOFactory.DAO_KEY);
        }

        public TenantRegistrator(IDAOFactory daoFactory)
        {
            dao = daoFactory.GetTenantDAO();
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

        public void ValidateTenantRegistrationInfo(TenantRegistrationInfo registrationInfo)
        {
            if (registrationInfo == null) throw new ArgumentNullException("registrationInfo");
            if (string.IsNullOrEmpty(registrationInfo.Name)) throw new Exception("Community name can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.Address)) throw new Exception("Community address can not be empty");

            if (string.IsNullOrEmpty(registrationInfo.Email)) throw new Exception("Account email can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.FirstName)) throw new Exception("Account firstname can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.LastName)) throw new Exception("Account lastname can not be empty");
            if (string.IsNullOrEmpty(registrationInfo.Password)) registrationInfo.Password = Crypto.GeneratePassword(6);
        }
    }
}
