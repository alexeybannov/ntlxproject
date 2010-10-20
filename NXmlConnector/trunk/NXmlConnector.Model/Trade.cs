using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Trade
    {
        [XmlElement("secid")]
        public string SecId
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

        [XmlElement("orderno")]
        public string OrderNo
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
        public string BuySell
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

        [XmlElement("brokerref")]
        public string BrokerRef
        {
            get;
            set;
        }

        [XmlElement("value")]
        public string Value
        {
            get;
            set;
        }

        [XmlElement("comission")]
        public string Comission
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
        public string Quantity
        {
            get;
            set;
        }

        [XmlElement("yield")]
        public string Yield
        {
            get;
            set;
        }

        [XmlElement("accruedint")]
        public string AccruedInt
        {
            get;
            set;
        }

        [XmlElement("tradetype")]
        public string TradeType
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