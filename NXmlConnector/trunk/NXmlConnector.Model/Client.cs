using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("client")]
    public class Client
    {
        [XmlAttribute("id")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute("remove")]
        public bool Remove
        {
            get;
            set;
        }

        [XmlElement("type")]
        public string Type
        {
            get;
            set;
        }

        [XmlElement("currency")]
        public string Currency
        {
            get;
            set;
        }

        [XmlElement("ml_intraday")]
        public int Intraday
        {
            get;
            set;
        }

        [XmlElement("ml_overnight")]
        public int Overnight
        {
            get;
            set;
        }

        [XmlElement("ml_restrict")]
        public string Restrict
        {
            get;
            set;
        }

        [XmlElement("ml_call")]
        public string Call
        {
            get;
            set;
        }

        [XmlElement("ml_close")]
        public string Close
        {
            get;
            set;
        }


        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            var objBase = obj as Client;
            return objBase != null && Id.Equals(objBase.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
