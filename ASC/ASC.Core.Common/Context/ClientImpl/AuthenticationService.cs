using System;
using System.Linq;
using System.Security.Principal;
using System.Text;
using ASC.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using UsersConst = ASC.Core.Users.Constants;

namespace ASC.Core
{
    public class AuthenticationService : IAuthenticationClient
    {
        public IUserAccount[] GetUserAccounts()
        {
            return CoreContext.UserManager.GetUsers(ASC.Core.Users.EmployeeStatus.Active)
                .Select(u => new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId))
                .ToArray();
        }

        public void SetUserPassword(Guid userID, string password)
        {
            CoreContext.InternalAuthentication.SetUserPassword(userID, password);
        }

        public string GetUserPasswordHash(Guid userID)
        {
            string pwdHash = CoreContext.InternalAuthentication.GetUserPasswordHash(userID);
            return pwdHash != null ? Encoding.Unicode.GetString(Convert.FromBase64String(pwdHash)) : null;
        }

        public IPrincipal AuthenticateAccount(IAccount account)
        {
            if (account == null) throw new ArgumentNullException("account");
            return CoreContext.InternalAuthentication.AuthenticateAccount(account);
        }

        public IAccount GetAccountByID(Guid id)
        {
            var u = CoreContext.UserManager.GetUsers(id);
            return !UsersConst.LostUser.Equals(u) ? new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId) : null;
        }
    }
}