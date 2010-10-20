using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("candles")]
    public class Candles
    {
        [XmlAttribute("secid")]
        public string SecurityId
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
        public int status;

        public CandlesStatus Status
        {
            get { return (CandlesStatus)status; }
        }

        [XmlElement("candle")]
        public Candle[] CandlesArray
        {
            get;
            set;
        }
    }
}
