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
        public double SaldoIn
        {
            get;
            set;
        }

        [XmlElement("bought")]
        public double Bought
        {
            get;
            set;
        }

        [XmlElement("sold")]
        public double Sold
        {
            get;
            set;
        }

        [XmlElement("saldo")]
        public double Saldo
        {
            get;
            set;
        }

        [XmlElement("ordbuy")]
        public double OrdBuy
        {
            get;
            set;
        }

        [XmlElement("ordbuycond")]
        public double OrdBuyCondition
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
    }
}