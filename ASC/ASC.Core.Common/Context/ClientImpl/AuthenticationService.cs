using System;
using System.Linq;
using ASC.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using ASC.Core.Users;
using UsersConst = ASC.Core.Users.Constants;

namespace ASC.Core
{
    public class AuthenticationService : IAuthenticationClient
    {
        private readonly IUserService userService;


        public AuthenticationService(IUserService userService)
        {
            this.userService = userService;
        }


        public IUserAccount[] GetUserAccounts()
        {
            return CoreContext.UserManager.GetUsers(ASC.Core.Users.EmployeeStatus.Active)
                .Select(u => new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId))
                .ToArray();
        }

        public void SetUserPassword(Guid userID, string password)
        {
            userService.SetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID, password);
        }

        public string GetUserPasswordHash(Guid userID)
        {
            return userService.GetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID);
        }

        public IAccount GetAccountByID(Guid id)
        {
            var u = CoreContext.UserManager.GetUsers(id);
            return !UsersConst.LostUser.Equals(u) && u.Status == EmployeeStatus.Active ?
                new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId) :
                null;
        }
    }
}