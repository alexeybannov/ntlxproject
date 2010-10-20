using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("error")]
    public class Error
    {
        [XmlElement("error")]
        public string ErrorText
        {
            get;
            set;
        }
    }
}
