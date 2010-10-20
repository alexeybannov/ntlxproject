using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class LeverageControlSecurity
    {
        [XmlAttribute("secid")]
        public string SecurityId
        {
            get;
            set;
        }

        [XmlAttribute("maxbuy")]
        public string MaxBuy
        {
            get;
            set;
        }

        [XmlAttribute("maxsell")]
        public string MaxSell
        {
            get;
            set;
        }
    }
}