using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Order
    {
        [XmlAttribute("transactionid")]
        public string TransactionId
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

        [XmlElement("secid")]
        public string SecurityId
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

        [XmlElement("status")]
        public string Status
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
        public string Value
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
        public string Balance
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
        public string ConditionValue
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
        public string MaxComission
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