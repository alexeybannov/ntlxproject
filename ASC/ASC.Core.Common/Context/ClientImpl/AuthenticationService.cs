#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Core.Common.Cache;
using ASC.Core.Configuration;
using UsersConst = ASC.Core.Users.Constants;

#endregion

namespace ASC.Core
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IDictionary<int, ICache<Guid, IUserAccount>> accountsCache;

        private ICache<Guid, IUserAccount> AccountsCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (accountsCache)
                {
                    if (!accountsCache.ContainsKey(tenant))
                    {
                        var accountsCacheInfo = new CacheInfo(UsersConst.CacheIdAccounts + tenant);
                        accountsCacheInfo.AddParentCache(UsersConst.CacheIdUsers + tenant, CacheReaction.Synchronize,
                                                         CacheReaction.Synchronize, CacheReaction.Synchronize);
                        accountsCache[tenant] =
                            CoreContext.CacheInfoStorage.CreateCache<Guid, IUserAccount>(accountsCacheInfo, SyncAccounts);
                    }
                    return accountsCache[tenant];
                }
            }
        }

        internal AuthenticationService()
        {
            accountsCache = new Dictionary<int, ICache<Guid, IUserAccount>>();
        }

        #region IService

        public IServiceInfo Info
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Implementation of IAuthentication

        public IUserAccount[] GetUserAccounts()
        {
            return AccountsCache.Values.ToArray();
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

        #endregion

        public IAccount GetAccountByID(Guid id)
        {
            return AccountsCache.ContainsKey(id) ? AccountsCache[id] : null;
        }

        private IDictionary<Guid, IUserAccount> SyncAccounts()
        {
            return CoreContext.InternalAuthentication.GetUserAccounts().ToDictionary(a => a.ID);
        }
    }
}