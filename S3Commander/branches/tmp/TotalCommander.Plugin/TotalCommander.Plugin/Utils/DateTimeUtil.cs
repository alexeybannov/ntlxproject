using System;
using System.Runtime.InteropServices.ComTypes;

namespace TotalCommander.Plugin.Utils
{
    static class DateTimeUtil
    {
        public static FILETIME ToFileTime(DateTime? dateTime)
        {
            var longTime = dateTime.HasValue ? dateTime.Value.ToFileTime() : long.MaxValue << 1;
            return new FILETIME()
            {
                dwHighDateTime = LongUtil.High(longTime),
                dwLowDateTime = LongUtil.Low(longTime),
            };
        }

        public static DateTime? FromFileTime(FILETIME fileTime)
        {
            var longTime = LongUtil.MakeLong(fileTime.dwHighDateTime, fileTime.dwLowDateTime);
            return longTime != 0 ? DateTime.FromFileTime(longTime) : (DateTime?)null;
        }
    }
}