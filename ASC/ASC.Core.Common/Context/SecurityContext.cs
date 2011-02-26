using System;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Security.Authentication;
using ASC.Core.Security.Authorizing;

namespace ASC.Core
{
    public static class SecurityContext
    {
        private static readonly object syncRoot = new object();
        private static IPermissionResolver permissionResolver;

        public static string SetUserPassword(Guid userID, string password)
        {
            CoreContext.Authentication.SetUserPassword(userID, password);
            if (AuthenticationContext.CurrentAccount.ID == userID)
            {
                return AuthenticationContext.ChangePassword(userID, password);
            }
            return null;
        }

        public static bool IsAuthenticated
        {
            get { return CurrentAccount.IsAuthenticated; }
        }

        public static void Logout()
        {
            AuthenticationContext.DoLogout();
        }

        public static string AuthenticateMe(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");
            return AuthenticationContext.DoLoginPasswordAuthentication(login, password);
        }

        public static void AuthenticateMe(IAccount account)
        {
            if (account == null) throw new ArgumentNullException("account");
            AuthenticationContext.DoAccountAuthentication(account);
        }

        public static bool AuthenticateMe(string cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");
            return AuthenticationContext.DoCookieAuthentication(cookie);
        }

        public static bool DemoMode
        {
            get { return IsAuthenticated && (CurrentAccount.ID == Constants.Demo.ID); }
        }

        public static bool DemoModeEnabled
        {
            get { return CoreContext.Configuration.Cfg.DemoAccountEnabled; }
        }

        public static IAccount CurrentAccount
        {
            get { return AuthenticationContext.CurrentAccount; }
        }

        public static IPermissionResolver PermissionResolver
        {
            get
            {
                if (permissionResolver == null)
                {
                    lock (syncRoot)
                    {
                        if (permissionResolver == null)
                        {
                            var permProvider = new PermissionProvider();
                            var roleProvider = new RoleProvider();
                            var azManager = new AzManager(roleProvider, permProvider);
                            permissionResolver = new PermissionResolver(azManager, permProvider);
                        }
                    }
                }
                return permissionResolver;
            }
        }

        public static bool CheckPermissions(params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, actions);
        }

        public static bool CheckPermissions<T>(object objectId, params IAction[] actions)
        {
            return CheckPermissions<T>(objectId, null, actions);
        }

        public static bool CheckPermissions<T>(object objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return CheckPermissions(new SecurityObjectId<T>(objectId), securityObjProvider, actions);
        }

        public static bool CheckPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(securityObject, null, actions);
        }

        public static bool CheckPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, objectId, securityObjProvider, actions);
        }

        public static void DemandPermissions(params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, actions);
        }

        public static void DemandPermissions<T>(object objectId, params IAction[] actions)
        {
            DemandPermissions<T>(objectId, null, actions);
        }

        public static void DemandPermissions<T>(object objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            DemandPermissions(new SecurityObjectId<T>(objectId), securityObjProvider, actions);
        }

        public static void DemandPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(securityObject, null, actions);
        }

        public static void DemandPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, objectId, securityObjProvider, actions);
        }
    }
}