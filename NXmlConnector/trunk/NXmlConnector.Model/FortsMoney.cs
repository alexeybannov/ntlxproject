using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class FortsMoney
    {
        [XmlElement("client")]
        public string Client
        {
            get;
            set;
        }

        [XmlElement("current")]
        public string Current
        {
            get;
            set;
        }

        [XmlElement("blocked")]
        public string Blocked
        {
            get;
            set;
        }

        [XmlElement("free")]
        public string Free
        {
            get;
            set;
        }

        [XmlElement("varmargin")]
        public string VarMargin
        {
            get;
            set;
        }
    }
}
