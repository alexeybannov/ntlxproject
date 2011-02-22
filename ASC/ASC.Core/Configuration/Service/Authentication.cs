using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration.Service;
using ASC.Core.Factories;
using ASC.Core.Security.Authentication;
using AuthConst = ASC.Common.Security.Authorizing.Constants;

[assembly: AssemblyServices(typeof(Authentication))]

namespace ASC.Core.Configuration.Service
{
    [Locator]
    class Authentication : RemotingServiceController, IAuthentication
    {
        private IDAOFactory daoFactory;

        internal Authentication(IDAOFactory daoFactory)
            : base(Constants.AuthenticationServiceInfo)
        {
            if (daoFactory == null) throw new ArgumentNullException("daoFactory");
            this.daoFactory = daoFactory;
        }

        #region IAuthentication

        public IPrincipal AuthenticateAccount(IAccount account)
        {
            if (account is ISysAccount && !Array.Exists(Constants.SystemAccounts, s => s.Equals(account)))
            {
                throw new SecurityException("Unknown system account.");
            }

            var roles = new List<string>() { Role.Everyone };

            if (account is IUserAccount)
            {
                var credential = ((UserAccount)account).Credential;
                if (credential == null) throw GetAuthError();

                var dao = daoFactory.GetConfigDao();

                account = dao.GetAccount(credential);
                if (account == null) throw GetAuthError();

                var groups = dao.GetAccountRoles(account.ID);
                if (groups.Contains(AuthConst.Admin.ID)) roles.Add(Role.Administrators);
                roles.Add(groups.Contains(AuthConst.Visitor.ID) ? Role.Visitors : Role.Users);
            }
            if (account is ISysAccount)
            {
                //TODO: check
                roles.Add(Role.System);
            }

            return new GenericPrincipal(account, roles.ToArray());
        }

        #endregion

        private Exception GetAuthError()
        {
            return new SecurityException("Invalid username or password.");
        }
    }
}