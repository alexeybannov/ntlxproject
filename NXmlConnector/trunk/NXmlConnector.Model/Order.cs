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
        public string time;

        public DateTime Time
        {
            get { return NXmlConverter.ToDateTime(time); }
        }

        [XmlElement("accepttime")]
        public string acceptTime;

        public DateTime AcceptTime
        {
            get { return NXmlConverter.ToDateTime(acceptTime); }
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

        [XmlElement("accruedint")]
        public double AccruedInt
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
        public string withDrawTime;

        public DateTime WithDrawTime
        {
            get { return NXmlConverter.ToDateTime(withDrawTime); }
        }

        [XmlElement("condition")]
        public OrderCondition Condition
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