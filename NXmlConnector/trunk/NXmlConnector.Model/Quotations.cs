using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("quotations")]
    public class Quotations
    {
        [XmlElement("quotation")]
        public _Quotation[] QuotationsArray
        {
            get;
            set;
        }
    }
}
