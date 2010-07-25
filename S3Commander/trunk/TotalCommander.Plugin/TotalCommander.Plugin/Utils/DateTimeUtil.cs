using System;
using System.Runtime.InteropServices.ComTypes;

namespace TotalCommander.Plugin.Utils
{
    static class DateTimeUtil
    {
        public static FILETIME ToFileTime(DateTime? dateTime)
        {
            var fileTime = new FILETIME();
            var longTime = dateTime.HasValue ? (ulong)dateTime.Value.ToFileTime() : 0xFFFFFFFFFFFFFFFE;
            fileTime.dwHighDateTime = LongUtil.High(longTime);
            fileTime.dwLowDateTime = LongUtil.Low(longTime);
            return fileTime;
        }

        public static DateTime? FromFileTime(FILETIME fileTime)
        {
            var longTime = LongUtil.MakeLong(fileTime.dwHighDateTime, fileTime.dwLowDateTime);
            if (longTime != 0) return DateTime.FromFileTime(longTime);
            return null;
        }
    }
}
