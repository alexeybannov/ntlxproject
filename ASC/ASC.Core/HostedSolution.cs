﻿using System.Collections.Generic;
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
	public class HostedSolution
	{
		private IDAOFactory daoFactory;

		private TenantRegistrator tenantRegistrator;

		private HostedSolution(IDAOFactory daoFactory)
		{
			this.daoFactory = daoFactory;
			tenantRegistrator = new TenantRegistrator(daoFactory.GetTenantDAO());
		}


		public static HostedSolution GetHostedSolution(ConnectionStringSettings connectionString)
		{
			if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
			{
				DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, connectionString);
			}
			return new HostedSolution(new DAOFactory());
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
			return daoFactory.GetTenantDAO().FindTenants(login, password);
		}

		public void CheckTenantAddress(string address)
		{
			tenantRegistrator.CheckTenantAddress(address);
		}

		public string RegisterTenant(TenantRegistrationInfo info)
		{
			return tenantRegistrator.RegisterTenant(info);
		}

		public string CreateAuthenticationCookie(int tenantID, string login, string password)
		{
			var credential = AuthenticationContext.CreateCredential(tenantID, login, password);
			return CookieStorage.Save(credential);
		}
	}
}