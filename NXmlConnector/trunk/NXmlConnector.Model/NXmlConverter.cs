using System;

namespace NXmlConnector.Model
{
    public static class NXmlConverter
    {
        public static string DateTimeFormat
        {
            get { return "dd.MM.yyyy HH:mm:ss"; }
        }

        public static DateTime ToDateTime(string datetime)
        {
            if (string.IsNullOrEmpty(datetime) || datetime == "0") return default(DateTime);
            if (datetime == "till_canceled") return DateTime.MaxValue;
            return DateTime.ParseExact(datetime, DateTimeFormat, null);
        }

        public static string ToString(DateTime? datetime)
        {
            return datetime.HasValue ? datetime.Value.ToString(NXmlConverter.DateTimeFormat) : "0";
        }
    }
}
