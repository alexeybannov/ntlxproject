using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Security
    {
        [XmlAttribute("secid")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute("active")]
        public bool Active
        {
            get;
            set;
        }

        [XmlElement("seccode")]
        public string Code
        {
            get;
            set;
        }

        [XmlElement("market")]
        public string Market
        {
            get;
            set;
        }

        [XmlElement("shortname")]
        public string ShoryName
        {
            get;
            set;
        }

        [XmlElement("decimals")]
        public int Decimals
        {
            get;
            set;
        }

        [XmlElement("minstep")]
        public string MinStep
        {
            get;
            set;
        }

        [XmlElement("lotsize")]
        public string LotSize
        {
            get;
            set;
        }

        [XmlElement("opmask")]
        public SecurityOptMask OpMask
        {
            get;
            set;
        }

        [XmlElement("sectype")]
        public string SecType
        {
            get;
            set;
        }


        public override string ToString()
        {
            return ShoryName + " - " + Code;
        }

        public override bool Equals(object obj)
        {
            var objBase = obj as Security;
            return objBase != null && Id.Equals(objBase.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
