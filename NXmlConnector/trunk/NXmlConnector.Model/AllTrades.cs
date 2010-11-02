using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("alltrades")]
    public class AllTrades
    {
        [XmlElement("trade")]
        public _AllTrade[] TradesArray
        {
            get;
            set;
        }
    }
}