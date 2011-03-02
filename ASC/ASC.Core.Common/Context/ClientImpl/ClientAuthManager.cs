using System;
using System.Linq;
using System.Security.Permissions;
using ASC.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using ASC.Core.Users;

namespace ASC.Core
{
    class ClientAuthManager : IAuthenticationClient
    {
        private readonly IUserService userService;


        public ClientAuthManager(IUserService userService)
        {
            this.userService = userService;
        }


        public IUserAccount[] GetUserAccounts()
        {
            return CoreContext.UserManager.GetUsers(EmployeeStatus.Active).Select(u => ToAccount(u)).ToArray();
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void SetUserPassword(Guid userID, string password)
        {
            if (SecurityContext.CurrentAccount.ID != userID) SecurityContext.DemandPermissions(ASC.Core.Configuration.Constants.Action_AuthSettings);

            userService.SetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID, password);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string GetUserPasswordHash(Guid userID)
        {
            if (SecurityContext.CurrentAccount.ID != userID) SecurityContext.DemandPermissions(ASC.Core.Configuration.Constants.Action_AuthSettings);

            return userService.GetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID);
        }

        public IAccount GetAccountByID(Guid id)
        {
            var u = CoreContext.UserManager.GetUsers(id);
            return !Constants.LostUser.Equals(u) && u.Status == EmployeeStatus.Active ? ToAccount(u) : null;
        }


        private IUserAccount ToAccount(UserInfo u)
        {
            return new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }
    }
}