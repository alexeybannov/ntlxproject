using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SecurityOptMask
    {
        [XmlAttribute("usecredit")]
        public string UseCredit
        {
            get;
            set;
        }

        [XmlAttribute("bymarket")]
        public string ByMarket
        {
            get;
            set;
        }

        [XmlAttribute("nosplit")]
        public string NoSplit
        {
            get;
            set;
        }

        [XmlAttribute("immorcancel")]
        public string ImmorCancel
        {
            get;
            set;
        }

        [XmlAttribute("cancelbalance")]
        public string CancelBalance
        {
            get;
            set;
        }
    }
}
