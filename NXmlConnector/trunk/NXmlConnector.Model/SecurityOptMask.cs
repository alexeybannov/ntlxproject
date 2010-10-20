using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SecurityOptMask
    {
        [XmlAttribute("usecredit")]
        public string useCredit;

        [XmlAttribute("bymarket")]
        public string byMarket;

        [XmlAttribute("nosplit")]
        public string noSplit;

        [XmlAttribute("immorcancel")]
        public string immOrCancel;

        [XmlAttribute("cancelbalance")]
        public string cancelBalance;


        public bool UseCredit
        {
            get { return NXmlConverter.ToBoolean(useCredit); }
        }

        public bool ByMarket
        {
            get { return NXmlConverter.ToBoolean(byMarket); }
        }

        public bool NoSplit
        {
            get { return NXmlConverter.ToBoolean(noSplit); }
        }

        public bool ImmOrCancel
        {
            get { return NXmlConverter.ToBoolean(immOrCancel); }
        }

        public bool CancelBalance
        {
            get { return NXmlConverter.ToBoolean(cancelBalance); }
        }
    }
}
