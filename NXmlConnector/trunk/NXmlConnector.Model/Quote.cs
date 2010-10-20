using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quote
    {
        [XmlAttribute("secid")]
        public string SecurityId
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

        [XmlElement("yield")]
        public string Yield
        {
            get;
            set;
        }

        [XmlElement("buy")]
        public string Buy
        {
            get;
            set;
        }

        [XmlElement("sell")]
        public string Sell
        {
            get;
            set;
        }
    }
}