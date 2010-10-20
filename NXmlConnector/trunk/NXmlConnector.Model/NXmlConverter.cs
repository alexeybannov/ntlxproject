using System;

namespace NXmlConnector.Model
{
    public static class NXmlConverter
    {
        public const string DATE_FORMAT = "dd.MM.yyyy HH:mm:ss";


        public static DateTime ToDateTime(string datetime)
        {
            return DateTime.ParseExact(datetime, DATE_FORMAT, null);
        }

        public static string ToString(OrderType orderType)
        {
            return orderType == OrderType.Buy ? "B" : "S";
        }

        public static string ToString(DateTime datetime)
        {
            return datetime.ToString(DATE_FORMAT);
        }
    }
}
