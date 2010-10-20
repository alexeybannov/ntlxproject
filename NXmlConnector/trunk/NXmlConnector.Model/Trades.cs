using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("trades")]
    public class Trades
    {
        [XmlElement("trade")]
        public Trade[] TradesArray
        {
            get;
            set;
        }
    }
}