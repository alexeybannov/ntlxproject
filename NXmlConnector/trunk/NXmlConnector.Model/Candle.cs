using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Candle
    {
        [XmlAttribute("date")]
        public DateTime Date
        {
            get;
            set;
        }

        [XmlAttribute("open")]
        public double Open
        {
            get;
            set;
        }

        [XmlAttribute("high")]
        public double High
        {
            get;
            set;
        }

        [XmlAttribute("low")]
        public double Low
        {
            get;
            set;
        }

        [XmlAttribute("close")]
        public double Close
        {
            get;
            set;
        }

        [XmlAttribute("volume")]
        public int Volume
        {
            get;
            set;
        }

        [XmlAttribute("oi")]
        public double OI
        {
            get;
            set;
        }
    }
}
