using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("candlekinds")]
    public class CandleKinds
    {
        [XmlElement("kind")]
        public CandleKind[] Kinds
        {
            get;
            set;
        }
    }
}
