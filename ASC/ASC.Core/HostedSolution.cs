using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using ASC.Common.Data;
using ASC.Common.Security.Authentication;
using ASC.Core.Common.Security.Authentication;
using ASC.Core.Factories;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

namespace ASC.Core
{
	public class HostedSolution
	{
		private IDAOFactory daoFactory;


		private HostedSolution(IDAOFactory daoFactory)
		{
			this.daoFactory = daoFactory;
		}

		public static HostedSolution GetHostedSolution(ConnectionStringSettings connectionString)
		{
			if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
			{
				DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, connectionString);
			}
			return new HostedSolution(new DAOFactory());
		}

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }
        
		public List<Tenant> FindTenants(string login, string password)
		{
            if (password != null)
            {
                var account = daoFactory
                    .GetConfigDao()
                    .GetAccount(new Credential(login, Hasher.Base64Hash(password, HashAlg.SHA256)));
                if (account == null) throw new SecurityException("Invalid login or password.");
            }
			return daoFactory.GetTenantDAO().GetTenants(login, password);
		}

		public void CheckTenantAddress(string address)
		{
            daoFactory.GetTenantDAO().CheckTenantAddress(address);
		}

        public string RegisterTenant(TenantRegistrationInfo ri)
        {
            ValidateTenantRegistrationInfo(ri);

            bool doLogout = true;
            try
            {
                SecurityContext.AuthenticateMe(Configuration.Constants.CoreSystem);

                var tenant = new Tenant(ri.Address.ToLowerInvariant())
                {
                    Name = ri.Name,
                    Language = ri.Culture.Name,
                    TimeZone = ri.TimeZoneInfo,
                    OwnerName = string.Format("{0} {1}", ri.FirstName, ri.LastName),
                    OwnerEMail = ri.Email
                };
                tenant = CoreContext.TenantManager.SaveTenant(tenant);
                CoreContext.TenantManager.SetCurrentTenant(tenant);

                var user = new UserInfo()
                {
                    LastName = ri.LastName,
                    FirstName = ri.FirstName,
                    Email = ri.Email,
                    UserName = ri.Email.Substring(0, ri.Email.IndexOf('@'))
                };
                user = CoreContext.UserManager.SaveUserInfo(user);
                CoreContext.Authentication.SetUserPassword(user.ID, ri.Password);
                CoreContext.UserManager.AddUserIntoGroup(user.ID, Constants.GroupAdmin.ID);

                SecurityContext.Logout();
                doLogout = false;
                return SecurityContext.AuthenticateMe(ri.Email, ri.Password);
            }
            finally
            {
                if (doLogout) SecurityContext.Logout();
            }
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