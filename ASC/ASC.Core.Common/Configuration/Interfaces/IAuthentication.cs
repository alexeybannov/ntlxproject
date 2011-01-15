#region usings

using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Configuration
{
    [Service("{005E6280-3852-403c-A768-8AF42A059269}")]
    public interface IAuthentication : IService
    {
        IUserAccount[] GetUserAccounts();
        void SetUserPassword(Guid userID, string password);
        string GetUserPasswordHash(Guid userID);
        IPrincipal AuthenticateAccount(IAccount account);
        IAccount GetAccountByID(Guid id);
    }
}