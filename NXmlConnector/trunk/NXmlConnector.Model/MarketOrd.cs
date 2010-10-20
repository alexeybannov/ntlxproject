using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("marketord")]
    public class MarketOrd
    {
        [XmlAttribute("secid")]
        public string SecurityId
        {
            get;
            set;
        }

        [XmlAttribute("permit")]
        public string Permit
        {
            get;
            set;
        }
    }
}
