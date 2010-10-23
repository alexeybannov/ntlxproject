using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quotation
    {
        [XmlAttribute("secid")]
        public int SecurityId
        {
            get;
            set;
        }

        [XmlElement("accruedintvalue")]
        public double AccruedIntValue
        {
            get;
            set;
        }

        [XmlElement("open")]
        public double Open
        {
            get;
            set;
        }

        [XmlElement("waprice")]
        public double Waprice
        {
            get;
            set;
        }

        [XmlElement("biddeptht")]
        public int BidDeptht
        {
            get;
            set;
        }

        [XmlElement("numbids")]
        public int NumBids
        {
            get;
            set;
        }

        [XmlElement("offerdeptht")]
        public int OfferDeptht
        {
            get;
            set;
        }

        [XmlElement("bid")]
        public double Bid
        {
            get;
            set;
        }

        [XmlElement("offer")]
        public double Offer
        {
            get;
            set;
        }

        [XmlElement("numoffers")]
        public int NumOffers
        {
            get;
            set;
        }

        [XmlElement("numtrades")]
        public int NumTrades
        {
            get;
            set;
        }

        [XmlElement("voltoday")]
        public int VolToday
        {
            get;
            set;
        }

        [XmlElement("openpositions")]
        public int OpenPositions
        {
            get;
            set;
        }

        [XmlElement("deltapositions")]
        public int DeltaPositions
        {
            get;
            set;
        }

        [XmlElement("last")]
        public double Last
        {
            get;
            set;
        }

        [XmlElement("quantity")]
        public int Quantity
        {
            get;
            set;
        }

        [XmlElement("time")]
        public string time;

        public DateTime Time
        {
            get { return NXmlConverter.ToDateTime(time); }
        }

        [XmlElement("change")]
        public double Change
        {
            get;
            set;
        }

        [XmlElement("priceminusprevwaprice")]
        public double PriceMinusPrevWaprice
        {
            get;
            set;
        }

        [XmlElement("valtoday")]
        public double ValToDay
        {
            get;
            set;
        }

        [XmlElement("yield")]
        public double Yield
        {
            get;
            set;
        }

        [XmlElement("yieldatwaprice")]
        public double YielDatWaprice
        {
            get;
            set;
        }

        [XmlElement("marketpricetoday")]
        public double MarketPriceToday
        {
            get;
            set;
        }

        [XmlElement("highbid")]
        public double HighBid
        {
            get;
            set;
        }

        [XmlElement("lowoffer")]
        public double LowOffer
        {
            get;
            set;
        }

        [XmlElement("high")]
        public double High
        {
            get;
            set;
        }

        [XmlElement("low")]
        public double Low
        {
            get;
            set;
        }

        [XmlElement("closeprice")]
        public double ClosePrice
        {
            get;
            set;
        }

        [XmlElement("closeyield")]
        public double CloseYield
        {
            get;
            set;
        }

        [XmlElement("status")]
        public QuotationStatus Status
        {
            get;
            set;
        }

        [XmlElement("tradingstatus")]
        public TradingStatus TradingStatus
        {
            get;
            set;
        }
    }
}