#region usings

using System;
using System.IO;

#endregion

namespace ASC.Common.Utils
{
    internal sealed class SysNameChecker
    {
        public static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static void Check(string sysName)
        {
            if (String.IsNullOrEmpty(sysName))
                throw new ArgumentNullException("sysName");
            CheckFileSystemName(sysName);
            CheckUriName(sysName);
        }

        public static void CheckFileSystemName(string sysName)
        {
            if (sysName == null) throw new ArgumentNullException("sysName");
            if (
                sysName.IndexOfAny(InvalidFileNameChars) != -1
                ||
                sysName.IndexOfAny(InvalidPathChars) != -1
                )
                throw new ArgumentException("Contains invalid characters for file system", "sysName");
        }

        public static void CheckUriName(string sysName)
        {
            if (sysName == null) throw new ArgumentNullException("sysName");
            if (!Uri.IsWellFormedUriString(sysName, UriKind.Relative))
                throw new ArgumentException("Not well formated for relative uri", "sysName");
        }
    }
}