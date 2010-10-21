using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SecurityPosition
    {
        [XmlElement("secid")]
        public int Id
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

        [XmlElement("saldo")]
        public int Saldo
        {
            get;
            set;
        }

        [XmlElement("saldoin")]
        public int SaldoIn
        {
            get;
            set;
        }

        [XmlElement("saldomin")]
        public int SaldoMin
        {
            get;
            set;
        }

        [XmlElement("bought")]
        public int Bought
        {
            get;
            set;
        }

        [XmlElement("sold")]
        public int Sold
        {
            get;
            set;
        }

        [XmlElement("ordbuy")]
        public int OrdBuy
        {
            get;
            set;
        }

        [XmlElement("ordsell")]
        public int OrdSell
        {
            get;
            set;
        }
    }
}