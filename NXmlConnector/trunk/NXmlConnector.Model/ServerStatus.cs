using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("server_status")]
    public class ServerStatus
    {
        [XmlAttribute("connected")]
        public string Status
        {
            get;
            set;
        }

        [XmlText]
        public string ErrorText
        {
            get;
            set;
        }
    }
}
