using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;

namespace ASC.Core.Configuration
{
    [Service("{005E6280-3852-403c-A768-8AF42A059269}")]
    interface IAuthentication : IService
    {
        IPrincipal AuthenticateAccount(IAccount account);
        
        string GetUserPasswordHash(Guid userID);

        void SetUserPassword(Guid userID, string password);
    }
}