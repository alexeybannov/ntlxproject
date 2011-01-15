#region usings

using System.Collections;

#endregion

namespace ASC.Common.Utils.Extentions
{
    public static class ObjectExtentions
    {
        public static bool In(this object obj, params object[] compares)
        {
            if (compares != null && compares.Length > 0)
                foreach (object compareto in compares)
                    if (Comparer.Default.Compare(obj, compareto) == 0) return true;
            return false;
        }
    }
}