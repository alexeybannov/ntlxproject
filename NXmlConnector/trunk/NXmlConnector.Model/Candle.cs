using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Candle
    {
        [XmlAttribute("date")]
        public string date;

        public DateTime Date
        {
            get { return NXmlConverter.ToDateTime(date); }
        }

        [XmlAttribute("open")]
        public string Open
        {
            get;
            set;
        }

        [XmlAttribute("high")]
        public string High
        {
            get;
            set;
        }

        [XmlAttribute("low")]
        public string Low
        {
            get;
            set;
        }

        [XmlAttribute("close")]
        public string Close
        {
            get;
            set;
        }

        [XmlAttribute("volume")]
        public string Volume
        {
            get;
            set;
        }

        [XmlAttribute("oi")]
        public int OI
        {
            get;
            set;
        }
    }
}
