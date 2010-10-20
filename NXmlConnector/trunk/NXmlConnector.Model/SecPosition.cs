using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SecPosition
    {
        [XmlElement("secid")]
        public string Id
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

        [XmlElement("saldomin")]
        public string SaldoMin
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

        [XmlElement("ordsell")]
        public string OrdSell
        {
            get;
            set;
        }
    }
}