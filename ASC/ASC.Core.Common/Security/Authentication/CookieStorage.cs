using System;
using System.Web;
using ASC.Core.Security.Authentication;
using ASC.Security.Cryptography;

namespace ASC.Core.Common.Security.Authentication
{
    class CookieStorage
    {
        public static Credential Get(string cookie)
        {
            try
            {
                string str = InstanceCrypto.Decrypt(cookie);
                string[] strs = str.Split('$');
                if (string.Compare(strs[3], GetUserDepenencySalt()) != 0) return null;
                return new Credential(strs[0], strs[2], Convert.ToInt32(strs[1]));
            }
            catch
            {
                return null;
            }
        }

        public static string Save(Credential loginPwd)
        {
            var str = String.Format("{0}${1}${2}${3}",
                                       loginPwd.Login.ToLowerInvariant(),
                                       loginPwd.Tenant,
                                       loginPwd.PasswordHash,
                                       GetUserDepenencySalt());
            return InstanceCrypto.Encrypt(str);
        }

        public static string Update(Credential loginPwd, string cookie)
        {
            return Save(loginPwd);
        }

        public static void Remove(string cookie)
        {
        }

        
        private static string GetUserDepenencySalt()
        {
            var data = string.Empty;
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    data = HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch { }
            return Hasher.Base64Hash(data, HashAlg.MD5);
        }
    }
}