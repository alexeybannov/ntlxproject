using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class AllTrade
    {
        [XmlAttribute("secid")]
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

        [XmlElement("time")]
        public string Time
        {
            get;
            set;
        }

        [XmlElement("board")]
        public string Board
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

        [XmlElement("buysell")]
        public string BuySell
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
    }
}