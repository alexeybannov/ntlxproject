using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("clientlimits")]
    public class ClientLimits
    {
        [XmlAttribute("client")]
        public string Client
        {
            get;
            set;
        }

        [XmlElement("cbplimit")]
        public string CbpLimit
        {
            get;
            set;
        }

        [XmlElement("cbplused")]
        public string CbplUsed
        {
            get;
            set;
        }

        [XmlElement("cbplplanned")]
        public string CbplPlanned
        {
            get;
            set;
        }

        [XmlElement("fob_varmargin")]
        public string Fob_VarMargin
        {
            get;
            set;
        }

        [XmlElement("coverage")]
        public string Coverage
        {
            get;
            set;
        }

        [XmlElement("liquidity_c")]
        public string LiquidityC
        {
            get;
            set;
        }

        [XmlElement("profit")]
        public string Profit
        {
            get;
            set;
        }

        [XmlElement("money_current")]
        public string MoneyCurrent
        {
            get;
            set;
        }

        [XmlElement("money_blocked")]
        public string MoneyBlocked
        {
            get;
            set;
        }

        [XmlElement("money_free")]
        public string MoneyFree
        {
            get;
            set;
        }

        [XmlElement("options_premium")]
        public string OptionsPremium
        {
            get;
            set;
        }

        [XmlElement("exchange_fee")]
        public string ExchangeFee
        {
            get;
            set;
        }

        [XmlElement("forts_varmargin")]
        public string FortsVarMargin
        {
            get;
            set;
        }

        [XmlElement("varmargin")]
        public string VarMargin
        {
            get;
            set;
        }

        [XmlElement("pclmargin")]
        public string PclMargin
        {
            get;
            set;
        }

        [XmlElement("options_vm")]
        public string OptionsVm
        {
            get;
            set;
        }

        [XmlElement("spot_buy_limit")]
        public string SpotBuyLimit
        {
            get;
            set;
        }

        [XmlElement("used_stop_buy_limit")]
        public string UsedStopBuyLimit
        {
            get;
            set;
        }

        [XmlElement("collat_current")]
        public string CollatCurrent
        {
            get;
            set;
        }

        [XmlElement("collat_blocked")]
        public string CollatBlocked
        {
            get;
            set;
        }

        [XmlElement("collat_free")]
        public string CollatFree
        {
            get;
            set;
        }
    }
}
