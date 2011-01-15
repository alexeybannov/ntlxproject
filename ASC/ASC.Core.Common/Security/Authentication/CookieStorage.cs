#region usings

using System;
using System.Web;
using ASC.Common.Security.Authentication;
using ASC.Security.Cryptography;

#endregion

namespace ASC.Core.Common.Security.Authentication
{
    internal class CookieStorage
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
            string str = String.Format("{0}${1}${2}${3}",
                                       loginPwd.Login.ToLowerInvariant(),
                                       loginPwd.Tenant,
                                       loginPwd.PasswordHash,
                                       GetUserDepenencySalt()
                );
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
            string data = string.Empty;
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    data = HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch
            {
                data = string.Empty;
            }
            return Hasher.Base64Hash(data, HashAlg.MD5);
        }
    }
}