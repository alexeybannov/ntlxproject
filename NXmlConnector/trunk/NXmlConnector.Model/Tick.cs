using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Tick
    {
        [XmlElement("secid")]
        public int SecurityId
        {
            get;
            set;
        }

        [XmlElement("tradeno")]
        public int TradeNo
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
        public double Price
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
        public TradingStatus Period
        {
            get;
            set;
        }

        [XmlElement("buysell")]
        public OrderType BuySell
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