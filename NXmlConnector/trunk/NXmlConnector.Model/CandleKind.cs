using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class CandleKind
    {
        [XmlElement("id")]
        public int Id
        {
            get;
            set;
        }

        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("period")]
        public int period;


        public TimeSpan Period
        {
            get { return TimeSpan.FromSeconds(period); }
        }


        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var objBase = obj as CandleKind;
            return objBase != null && Id.Equals(objBase.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
