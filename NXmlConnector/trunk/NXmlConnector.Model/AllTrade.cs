using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class AllTrade
    {
        [XmlAttribute("secid")]
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

        [XmlElement("time")]
        public string time;

        public DateTime Time
        {
            get { return NXmlConverter.ToDateTime(time); }
        }

        [XmlElement("board")]
        public string Board
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

        [XmlElement("buysell")]
        public OrderType BuySell
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

        [XmlElement("openinterest")]
        public string OpenInterest
        {
            get;
            set;
        }
    }
}