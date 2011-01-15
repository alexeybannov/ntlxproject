#region usings

using System;
using System.Configuration;
using System.IO;
using System.Text;

#endregion

namespace ASC.Security.Cryptography
{
    public static class MachinePseudoKeys
    {
        public static byte[] GetMachineConstant()
        {
            string confkey = ConfigurationManager.AppSettings["asc.common.machinekey"];
            if (!String.IsNullOrEmpty(confkey))
            {
                return Encoding.UTF8.GetBytes(confkey);
            }
            else
            {
                string path = typeof (MachinePseudoKeys).Assembly.Location;
                var fi = new FileInfo(path);
                return BitConverter.GetBytes(fi.CreationTime.ToOADate());
            }
        }

        public static byte[] GetMachineConstant(int bytesCount)
        {
            byte[] cnst = GetMachineConstant();
            int icnst = BitConverter.ToInt32(cnst, cnst.Length - sizeof (int));
            var rnd = new Random(icnst);
            var buff = new byte[bytesCount];
            rnd.NextBytes(buff);
            return buff;
        }
    }
}