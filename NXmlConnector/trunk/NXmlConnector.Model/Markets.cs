using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("markets")]
    public class Markets
    {
        [XmlElement("market")]
        public Market[] MarketsArray
        {
            get;
            set;
        }
    }
}
