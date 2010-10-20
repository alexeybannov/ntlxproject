using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("quotations")]
    public class Quotations
    {
        [XmlElement("quotation")]
        public Quotation[] QuotationsArray
        {
            get;
            set;
        }
    }
}
