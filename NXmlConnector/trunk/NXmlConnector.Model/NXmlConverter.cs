using System;

namespace NXmlConnector.Model
{
    public static class NXmlConverter
    {
        public const string DATE_FORMAT = "dd.MM.yyyy HH:mm:ss";

        public static DateTime ToDateTime(string datetime)
        {
            if (string.IsNullOrEmpty(datetime) || datetime == "0") return default(DateTime);
            if (datetime == "till_canceled") return DateTime.MaxValue;
            return DateTime.ParseExact(datetime, DATE_FORMAT, null);
        }

        public static string ToString(DateTime datetime)
        {
            return datetime.ToString(DATE_FORMAT);
        }

        public static bool ToBoolean(string value)
        {
            var result = true;
            if (bool.TryParse(value, out result)) return result;
            return value == "yes";
        }
    }
}
