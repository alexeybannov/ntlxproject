using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class MoneyPosition
    {
        [XmlElement("asset")]
        public string Asset
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

        [XmlElement("shortname")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("saldoin")]
        public string SaldoIn
        {
            get;
            set;
        }

        [XmlElement("bought")]
        public string Bought
        {
            get;
            set;
        }

        [XmlElement("sold")]
        public string Sold
        {
            get;
            set;
        }

        [XmlElement("saldo")]
        public string Saldo
        {
            get;
            set;
        }

        [XmlElement("ordbuy")]
        public string OrdBuy
        {
            get;
            set;
        }

        [XmlElement("ordbuycond")]
        public string OrdBuyCond
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
    }
}