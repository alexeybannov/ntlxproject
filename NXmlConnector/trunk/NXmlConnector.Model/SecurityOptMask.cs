using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class SecurityOptMask
    {
        [XmlAttribute("usecredit")]
        public YesNo useCredit;

        [XmlAttribute("bymarket")]
        public YesNo byMarket;

        [XmlAttribute("nosplit")]
        public YesNo noSplit;

        [XmlAttribute("immorcancel")]
        public YesNo immOrCancel;

        [XmlAttribute("cancelbalance")]
        public YesNo cancelBalance;


        public bool UseCredit
        {
            get { return useCredit == YesNo.yes; }
        }

        public bool ByMarket
        {
            get { return byMarket == YesNo.yes; }
        }

        public bool NoSplit
        {
            get { return noSplit == YesNo.yes; }
        }

        public bool ImmOrCancel
        {
            get { return immOrCancel == YesNo.yes; }
        }

        public bool CancelBalance
        {
            get { return cancelBalance == YesNo.yes; }
        }
    }
}
