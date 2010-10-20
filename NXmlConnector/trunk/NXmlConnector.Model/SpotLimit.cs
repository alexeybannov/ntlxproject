using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SpotLimit
    {
        [XmlElement("client")]
        public string Client
        {
            get;
            set;
        }

        [XmlElement("buylimit")]
        public string BuyLimit
        {
            get;
            set;
        }

        [XmlElement("buylimitused")]
        public string BuyLimitUsed
        {
            get;
            set;
        }
    }
}