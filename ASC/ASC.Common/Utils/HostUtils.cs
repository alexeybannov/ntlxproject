#region usings

using System.Web;

#endregion

namespace ASC.Common.Utils
{
    public sealed class HostUtils
    {
        public static bool IsWeb
        {
            get { return HttpContext.Current != null; }
        }
    }
}