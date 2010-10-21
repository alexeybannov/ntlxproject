using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quote
    {
        [XmlAttribute("secid")]
        public int SecurityId
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

        [XmlElement("yield")]
        public double Yield
        {
            get;
            set;
        }

        [XmlElement("buy")]
        public double Buy
        {
            get;
            set;
        }

        [XmlElement("sell")]
        public double Sell
        {
            get;
            set;
        }
    }
}