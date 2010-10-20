using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("overnight")]
    public class Overnight
    {
        [XmlAttribute("status")]
        public bool Status
        {
            get;
            set;
        }
    }
}
