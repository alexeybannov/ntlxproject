using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("marketord")]
    public class MarketOrd
    {
        [XmlAttribute("secid")]
        public int SecurityId
        {
            get;
            set;
        }

        [XmlAttribute("permit")]
        public YesNo permit;

        public bool Permit
        {
            get { return permit == YesNo.yes; }
        }
    }
}
