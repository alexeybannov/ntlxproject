#region usings

using System;
using System.Text;

#endregion

namespace ASC.Common.Utils
{
    public static class UriUtil
    {
        #region formatters

        private static readonly string _UriPartsSeparator = @"/";

        public static string BuildUri(string postfix, params string[] parts)
        {
            var sb = new StringBuilder(String.Join(_UriPartsSeparator, parts));
            sb.Append(postfix);
            return sb.ToString();
        }

        #endregion
    }
}