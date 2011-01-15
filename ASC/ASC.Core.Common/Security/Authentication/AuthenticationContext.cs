#region usings

using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using ASC.Common.Security.Authentication;
using ASC.Core.Common.Security.Authentication;
using ASC.Security.Cryptography;
using AuthConst = ASC.Core.Configuration.Constants;

#endregion

namespace ASC.Core.Security.Authentication
{
    internal static class AuthenticationContext
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
            get { return Principal.Identity is IAccount ? (IAccount) Principal.Identity : AuthConst.Guest; }
        }

        public static void DoAccountAuthentication(IAccount account)
        {
            Principal = CoreContext.InternalAuthentication.AuthenticateAccount(account);
        }

        public static string DoLoginPasswordAuthentication(string login, string password)
        {
            Credential credential = CreateCredential(CoreContext.TenantManager.GetCurrentTenant().TenantId, login,
                                                     password);
            DoAccountAuthentication(new UserAccount(credential));
            string cookie = CookieStorage.Save(credential);
            WebCookie = cookie;
            return cookie;
        }

        public static bool DoCookieAuthentication(string cookie)
        {
            Credential credential = CookieStorage.Get(cookie);
            if (credential == null ||
                credential.Tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId) return false;
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

        public static void DoLogout()
        {
            if (WebCookie != null) CookieStorage.Remove(WebCookie);
            WebCookie = null;
            Principal = null;
        }

        public static string ChangePassword(Guid userID, string password)
        {
            Credential credential = CreateCredential(CoreContext.TenantManager.GetCurrentTenant().TenantId,
                                                     userID.ToString(), password);
            return CookieStorage.Update(credential, WebCookie);
        }

        public static Credential CreateCredential(int tenantID, string login, string password)
        {
            return new Credential(
                login,
                Hasher.Base64Hash(password, HashAlg.SHA256),
                tenantID
                );
        }

        private static string WebCookie
        {
            get { return GetFromHttpSession<string>(AUTH_COOKIE); }
            set { SetToHttpSession(AUTH_COOKIE, value); }
        }

        private static T GetFromHttpSession<T>(string name)
        {
            return HttpContext.Current != null && HttpContext.Current.Session != null
                       ? (T) HttpContext.Current.Session[name]
                       : default(T);
        }

        private static void SetToHttpSession(string name, object obj)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[name] = obj;
            }
        }
    }
}