using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class FortsPosition
    {
        [XmlElement("secid")]
        public int SecurityId
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
        public double StartNet
        {
            get;
            set;
        }

        [XmlElement("saldoin")]
        public double SaldoIn
        {
            get;
            set;
        }

        [XmlElement("openbuys")]
        public double OpenBuys
        {
            get;
            set;
        }

        [XmlElement("opensells")]
        public double OpenSells
        {
            get;
            set;
        }

        [XmlElement("totalnet")]
        public double TotalNet
        {
            get;
            set;
        }

        [XmlElement("todaybuy")]
        public double TodayBuy
        {
            get;
            set;
        }

        [XmlElement("todaysell")]
        public double TodaySell
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
        public string NetTo
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