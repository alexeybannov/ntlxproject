using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Common.Security.Authentication;
using ASC.Security.Cryptography;
using AuthConst = ASC.Core.Configuration.Constants;
using UserConst = ASC.Core.Users.Constants;


namespace ASC.Core.Security.Authentication
{
    static class AuthenticationContext
    {
        private const string AUTH_PRINCIPAL = "__Auth.Principal";
        private const string AUTH_COOKIE = "__Auth.Cookie";

        public static IPrincipal Principal
        {
            get
            {
                var principal = GetFromHttpSession<IPrincipal>(AUTH_PRINCIPAL);
                if (principal != null)
                {
                    HttpContext.Current.User = principal;
                    Thread.CurrentPrincipal = principal;
                }
                return Thread.CurrentPrincipal;
            }
            set
            {
                SetToHttpSession(AUTH_PRINCIPAL, value);
                if (HttpContext.Current != null) HttpContext.Current.User = value;
                Thread.CurrentPrincipal = value;
            }
        }

        public static IAccount CurrentAccount
        {
            get { return Principal.Identity is IAccount ? (IAccount)Principal.Identity : AuthConst.Guest; }
        }


        public static string DoLoginPasswordAuthentication(string login, string password)
        {
            var credentila = CreateCredential(CoreContext.TenantManager.GetCurrentTenant().TenantId, login, password);
            DoAccountAuthentication(new UserAccount(credentila));

            var cookie = CookieStorage.Save(credentila);
            WebCookie = cookie;
            return cookie;
        }

        public static bool DoCookieAuthentication(string cookie)
        {
            var credential = CookieStorage.Get(cookie);
            if (credential == null || credential.Tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId) return false;
            try
            {
                DoAccountAuthentication(new UserAccount(credential));
                WebCookie = cookie;
            }
            catch
            {
                CookieStorage.Remove(cookie);
                WebCookie = null;
                return false;
            }
            return true;
        }

        public static void DoAccountAuthentication(IAccount account)
        {
            var roles = new List<string>() { Role.Everyone };

            if (account is ISysAccount)
            {
                if (Array.Exists(AuthConst.SystemAccounts, s => s.Equals(account)))
                {
                    roles.Add(Role.System);//TODO: check
                }
                else
                {
                    throw new SecurityException("Unknown system account.");
                }
            }

            if (account is IUserAccount)
            {
                var credential = ((UserAccount)account).GetCredential();
                if (credential == null) throw GetAuthError();

                var u = CoreContext.UserManager.GetUsers(credential.Tenant, credential.Login, credential.PasswordHash);
                if (u.ID == ASC.Core.Users.Constants.LostUser.ID) throw GetAuthError();

                if (CoreContext.UserManager.IsUserInGroup(u.ID, UserConst.GroupAdmin.ID)) roles.Add(Role.Administrators);
                roles.Add(CoreContext.UserManager.IsUserInGroup(u.ID, UserConst.GroupVisitor.ID) ? Role.Visitors : Role.Users);

                account = new UserAccount(u, credential.Tenant);
            }

            Principal = new GenericPrincipal(account, roles.ToArray());
        }


        public static void DoLogout()
        {
            if (WebCookie != null) CookieStorage.Remove(WebCookie);
            WebCookie = null;
            Principal = null;
        }

        public static string ChangePassword(Guid userID, string password)
        {
            var credential = CreateCredential(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID.ToString(), password);
            return CookieStorage.Update(credential, WebCookie);
        }

        public static Credential CreateCredential(int tenantID, string login, string password)
        {
            return new Credential(login, Hasher.Base64Hash(password, HashAlg.SHA256), tenantID);
        }


        private static string WebCookie
        {
            get { return GetFromHttpSession<string>(AUTH_COOKIE); }
            set { SetToHttpSession(AUTH_COOKIE, value); }
        }

        private static T GetFromHttpSession<T>(string name)
        {
            return HttpContext.Current != null && HttpContext.Current.Session != null
                       ? (T)HttpContext.Current.Session[name]
                       : default(T);
        }

        private static void SetToHttpSession(string name, object obj)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[name] = obj;
            }
        }

        private static Exception GetAuthError()
        {
            return new SecurityException("Invalid username or password.");
        }
    }
}