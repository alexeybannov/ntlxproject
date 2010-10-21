using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Trade
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

        [XmlElement("orderno")]
        public int OrderNo
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

        [XmlElement("client")]
        public string Client
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

        [XmlElement("time")]
        public string time;

        public DateTime Time
        {
            get { return NXmlConverter.ToDateTime(time); }
        }

        [XmlElement("brokerref")]
        public string BrokerRef
        {
            get;
            set;
        }

        [XmlElement("value")]
        public double Value
        {
            get;
            set;
        }

        [XmlElement("comission")]
        public double Comission
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

        [XmlElement("yield")]
        public double Yield
        {
            get;
            set;
        }

        [XmlElement("accruedint")]
        public double AccruedInt
        {
            get;
            set;
        }

        [XmlElement("tradetype")]
        public TradeType TradeType
        {
            get;
            set;
        }

        [XmlElement("settlecode")]
        public string SettleCode
        {
            get;
            set;
        }

        [XmlElement("currentpos")]
        public string CurrentPos
        {
            get;
            set;
        }
    }
}