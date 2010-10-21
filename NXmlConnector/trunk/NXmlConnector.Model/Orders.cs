using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("orders")]
    public class Orders
    {
        [XmlElement("order")]
        public Order[] OrderArray
        {
            get;
            set;
        }
    }
}