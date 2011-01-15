#region usings

using System;
using System.Text;

#endregion

namespace ASC.Security.Cryptography
{
    public class EmailValidationKeyProvider
    {
        #region public

        private static readonly DateTime _from = new DateTime(2010, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        private static byte[] _MachineConstant;

        public static string GetEmailKey(string email)
        {
            if (String.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            var ms = (long) (DateTime.UtcNow - _from).TotalMilliseconds;

            byte[] hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));

            string key = String.Format("{0}.{1}", ms, DoStringFromBytes(hash));

            return key;
        }

        public static bool ValidateEmailKey(string email, string key)
        {
            return ValidateEmailKey(email, key, TimeSpan.MaxValue);
        }

        public static bool ValidateEmailKey(string email, string key, TimeSpan validInterval)
        {
            if (String.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");
            if (key == null)
                throw new ArgumentNullException("key");
            string[] parts = key.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;

            long ms = 0;
            if (!Int64.TryParse(parts[0], out ms))
                return false;

            byte[] hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
            string key2 = DoStringFromBytes(hash);
            bool key2_good = String.Compare(parts[1], key2, StringComparison.InvariantCultureIgnoreCase) == 0;
            if (!key2_good)
                return false;
            var ms_current = (long) (DateTime.UtcNow - _from).TotalMilliseconds;
            return validInterval >= TimeSpan.FromMilliseconds(ms_current - ms);
        }

        public static byte[] GetMachineConstant()
        {
            if (_MachineConstant == null)
                _MachineConstant = GetMachineConstantInternal();
            return _MachineConstant;
        }

        #endregion

        internal static string DoStringFromBytes(byte[] data)
        {
            string str = Convert.ToBase64String(data);
            str = str.Replace("=", "").Replace("+", "").Replace("/", "").Replace("\\", "");
            return str.ToUpperInvariant();
        }

        internal static byte[] GetMashineHashedData(byte[] salt, byte[] data)
        {
            var alldata = new byte[salt.Length + data.Length];
            Array.Copy(data, alldata, data.Length);
            Array.Copy(salt, 0, alldata, data.Length, salt.Length);
            return Hasher.Hash(alldata, HashAlg.MD5);
        }

        internal static byte[] GetMachineConstantInternal()
        {
            return MachinePseudoKeys.GetMachineConstant();
        }
    }
}