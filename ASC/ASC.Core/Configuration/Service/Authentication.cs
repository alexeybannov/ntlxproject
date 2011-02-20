using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration.DAO;
using ASC.Core.Configuration.Service;
using ASC.Core.Factories;
using ASC.Core.Security.Authentication;
using ASC.Core.Users;
using ASC.Security.Cryptography;
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

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void SetUserPassword(Guid userID, string password)
        {
            if (SecurityContext.CurrentAccount.ID != userID)
            {
                SecurityContext.DemandPermissions(Constants.Action_AuthSettings);
            }

            var userSecurity = daoFactory.GetConfigDao().GetUserSecurity(userID);
            if (userSecurity == null)
            {
                userSecurity = new UserSecurity(userID);
            }

            userSecurity.PasswordHash = Hasher.Base64Hash(password, HashAlg.SHA256);
            userSecurity.PasswordHashSHA512 = GetHash512(password, true);

            try
            {
                daoFactory.GetConfigDao().SaveUserSecurity(userSecurity);
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string GetUserPasswordHash(Guid userID)
        {
            if (SecurityContext.CurrentAccount.ID != userID && !(SecurityContext.CurrentAccount is IServiceAccount))
            {
                SecurityContext.DemandPermissions(Constants.Action_AuthSettings);
            }

            var userSecurity = daoFactory.GetConfigDao().GetUserSecurity(userID);
            if (userSecurity == null)
            {
                throw new UserNotFoundException(userID);
            }

            var pwdHash = GetHash512(userSecurity.PasswordHashSHA512, false);
            return pwdHash != null ? Convert.ToBase64String(Encoding.Unicode.GetBytes(pwdHash)) : null;
        }

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
            if (account is ISysAccount || account is IServiceAccount)
            {
                //TODO: check
                roles.Add(Role.System);
            }

            return new GenericPrincipal(account, roles.ToArray());
        }

        #endregion

        private static string GetHash512(string data, bool revers)
        {
            if (string.IsNullOrEmpty(data)) return null;
            return Crypto.GetV(data, 1, revers);
        }

        private Exception GetAuthError()
        {
            return new SecurityException("Invalid username or password.");
        }
    }
}