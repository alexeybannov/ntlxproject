using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("market")]
    public class Market
    {
        [XmlAttribute("id")]
        public int Id
        {
            get;
            set;
        }

        [XmlText]
        public string Name
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var objBase = obj as Market;
            return objBase != null && Id.Equals(objBase.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
