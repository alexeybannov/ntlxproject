using System;

namespace ASC.Common.Utils.Extentions
{
    public static class DateTimeHelper
    {
        public static string ToShortString(this DateTime targetDateTime)
        {
            return String.Format("{0} {1}", targetDateTime.ToShortDateString(), targetDateTime.ToShortTimeString());
        }
    }
}