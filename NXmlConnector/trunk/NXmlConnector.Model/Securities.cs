using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("securities")]
    public class Securities
    {
        [XmlElement("security")]
        public Security[] SecuritiesArray
        {
            get;
            set;
        }
    }
}
