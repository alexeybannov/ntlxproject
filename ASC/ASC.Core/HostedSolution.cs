using System.Collections.Generic;
using System.Configuration;
using System.Security;
using ASC.Common.Data;
using ASC.Common.Security.Authentication;
using ASC.Core.Common.Security.Authentication;
using ASC.Core.Factories;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;

namespace ASC.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class HostedSolution
	{
		private IDAOFactory daoFactory;

		private TenantRegistrator tenantRegistrator;

		private HostedSolution(IDAOFactory daoFactory)
		{
			this.daoFactory = daoFactory;
			tenantRegistrator = new TenantRegistrator(daoFactory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public static HostedSolution GetHostedSolution(ConnectionStringSettings connectionString)
		{
			if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
			{
				DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, connectionString);
			}
			return new HostedSolution(new DAOFactory());
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }
        
        /// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public List<Tenant> FindTenants(string login, string password)
		{
            if (password != null)
            {
                var account = daoFactory
                    .GetConfigDao()
                    .GetAccount(new Credential(login, Hasher.Base64Hash(password, HashAlg.SHA256)));
                if (account == null) throw new SecurityException("Invalid login or password.");
            }
			return daoFactory.GetTenantDAO().FindTenants(login, password);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		public void CheckTenantAddress(string address)
		{
			tenantRegistrator.CheckTenantAddress(address);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registrationInfo"></param>
		/// <returns></returns>
		public string RegisterTenantInterim(TenantRegistrationInfo registrationInfo)
		{
			return tenantRegistrator.RegisterTenantInterim(registrationInfo);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tenantInterimKey"></param>
		public string RegisterTenant(string tenantInterimKey)
		{
			return RegisterTenant(tenantInterimKey, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tenantInterimKey"></param>
		/// <param name="templatesPath"></param>
		public string RegisterTenant(string tenantInterimKey, string templatesPath)
		{
			return tenantRegistrator.RegisterTenant(tenantInterimKey, templatesPath);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tenantID"></param>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public string CreateAuthenticationCookie(int tenantID, string login, string password)
		{
			var credential = AuthenticationContext.CreateCredential(tenantID, login, password);
			return CookieStorage.Save(credential);
		}
	}
}