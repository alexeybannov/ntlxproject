using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;

namespace ASC.Core
{
    public interface IAuthenticationClient
    {
        IUserAccount[] GetUserAccounts();

        IPrincipal AuthenticateAccount(IAccount account);

        IAccount GetAccountByID(Guid id);

        string GetUserPasswordHash(Guid userID);

        void SetUserPassword(Guid userID, string password);
    }
}