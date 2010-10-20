using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Order
    {
        [XmlAttribute("transactionid")]
        public int TransactionId
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

        [XmlElement("secid")]
        public int SecurityId
        {
            get;
            set;
        }

        [XmlElement("board")]
        public int Board
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

        [XmlElement("status")]
        public OrderStatus Status
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
        public string Time
        {
            get;
            set;
        }

        [XmlElement("accepttime")]
        public string AcceptTime
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
        public int Value
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

        [XmlElement("settlecode")]
        public string SettleCode
        {
            get;
            set;
        }

        [XmlElement("balance")]
        public double Balance
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

        [XmlElement("withdrawtime")]
        public string WithDrawTime
        {
            get;
            set;
        }

        [XmlElement("condition")]
        public string Condition
        {
            get;
            set;
        }

        [XmlElement("conditionvalue")]
        public double ConditionValue
        {
            get;
            set;
        }

        [XmlElement("validafter")]
        public string ValidAfter
        {
            get;
            set;
        }

        [XmlElement("validbefore")]
        public string ValidBefore
        {
            get;
            set;
        }

        [XmlElement("maxcomission")]
        public double MaxComission
        {
            get;
            set;
        }

        [XmlElement("result")]
        public string Result
        {
            get;
            set;
        }
    }
}