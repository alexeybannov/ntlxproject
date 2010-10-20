using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Tick
    {
        [XmlElement("secid")]
        public string SecurityId
        {
            get;
            set;
        }

        [XmlElement("tradeno")]
        public string TradeNo
        {
            get;
            set;
        }

        [XmlElement("tradetime")]
        public string TradeTime
        {
            get;
            set;
        }

        [XmlElement("price")]
        public string Price
        {
            get;
            set;
        }

        [XmlElement("quantity")]
        public int Quantity
        {
            get;
            set;
        }

        [XmlElement("period")]
        public string Period
        {
            get;
            set;
        }

        [XmlElement("buysell")]
        public string BuySell
        {
            get;
            set;
        }

        [XmlElement("openinterest")]
        public string OpenInterest
        {
            get;
            set;
        }
    }
}