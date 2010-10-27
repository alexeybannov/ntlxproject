using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Security
    {
        [XmlAttribute("secid")]
        public int Id
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
        public int Market
        {
            get;
            set;
        }

        [XmlElement("shortname")]
        public string Name
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
        public double MinStep
        {
            get;
            set;
        }

        [XmlElement("lotsize")]
        public int LotSize
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
        public SecurityType SecurityType
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool Permit
        {
            get;
            set;
        }


        public Security()
        {
            Permit = true;
        }

        public override string ToString()
        {
            return Name;
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
