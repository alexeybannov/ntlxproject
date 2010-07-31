using System;
using System.Text;

namespace TotalCommander.Plugin.Utils
{
    static class StringUtil
    {
        public static string First(string str, int length)
        {
            if (str == null || str.Length <= length) return str;
            return str.Substring(0, length);
        }

        public static string ToAnsi(string str, int maxLength)
        {
            if (str == null) return null;
            str = First(str, maxLength);
            var bytes = Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(str));
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
