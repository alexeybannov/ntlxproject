using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("ticks")]
    public class Ticks
    {
        [XmlElement("tick")]
        public Tick[] TickArray
        {
            get;
            set;
        }
    }
}