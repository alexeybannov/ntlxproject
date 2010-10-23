using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("candles")]
    public class Candles
    {
        [XmlAttribute("secid")]
        public int SecurityId
        {
            get;
            set;
        }

        [XmlAttribute("period")]
        public int Period
        {
            get;
            set;
        }

        [XmlAttribute("status")]
        public int Status
        {
            get;
            set;
        }

        [XmlElement("candle")]
        public Candle[] CandlesArray
        {
            get;
            set;
        }
    }
}
