using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class FortsPosition
    {
        [XmlElement("secid")]
        public string Id
        {
            get;
            set;
        }

        [XmlElement("client")]
        public string Client
        {
            get;
            set;
        }

        [XmlElement("startnet")]
        public string StartNet
        {
            get;
            set;
        }

        [XmlElement("saldoin")]
        public string SaldoIn
        {
            get;
            set;
        }

        [XmlElement("openbuys")]
        public string OpenBuys
        {
            get;
            set;
        }

        [XmlElement("opensells")]
        public string OpenSells
        {
            get;
            set;
        }

        [XmlElement("totalnet")]
        public string TotalNet
        {
            get;
            set;
        }

        [XmlElement("todaybuy")]
        public string TodayBuy
        {
            get;
            set;
        }

        [XmlElement("todaysell")]
        public string TodaySell
        {
            get;
            set;
        }

        [XmlElement("optmargin")]
        public string OptMargin
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

        [XmlElement("expirationpos")]
        public string ExpirationPos
        {
            get;
            set;
        }

        [XmlElement("usedsellspotlimit")]
        public string UsedSellSpotLimit
        {
            get;
            set;
        }

        [XmlElement("sellspotlimit")]
        public string SellSpotLimit
        {
            get;
            set;
        }

        [XmlElement("netto")]
        public string Netto
        {
            get;
            set;
        }

        [XmlElement("kgo")]
        public string Kgo
        {
            get;
            set;
        }
    }
}