using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quotation
    {
        [XmlAttribute("secid")]
        public string SecurityId
        {
            get;
            set;
        }

        [XmlElement("accruedintvalue")]
        public string AccruedIntValue
        {
            get;
            set;
        }

        [XmlElement("open")]
        public string Open
        {
            get;
            set;
        }

        [XmlElement("waprice")]
        public string Waprice
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
        public string Bid
        {
            get;
            set;
        }

        [XmlElement("offer")]
        public string Offer
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
        public string Last
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
        public string Time
        {
            get;
            set;
        }

        [XmlElement("change")]
        public string Change
        {
            get;
            set;
        }

        [XmlElement("priceminusprevwaprice")]
        public string PriceMinusPrevWaprice
        {
            get;
            set;
        }

        [XmlElement("valtoday")]
        public int ValToDay
        {
            get;
            set;
        }

        [XmlElement("yield")]
        public string Yield
        {
            get;
            set;
        }

        [XmlElement("yieldatwaprice")]
        public string YielDatWaprice
        {
            get;
            set;
        }

        [XmlElement("marketpricetoday")]
        public string MarketPriceToday
        {
            get;
            set;
        }

        [XmlElement("highbid")]
        public string HighBid
        {
            get;
            set;
        }

        [XmlElement("lowoffer")]
        public string LowOffer
        {
            get;
            set;
        }

        [XmlElement("sectype")]
        public string SecType
        {
            get;
            set;
        }

        [XmlElement("high")]
        public string High
        {
            get;
            set;
        }

        [XmlElement("low")]
        public string Low
        {
            get;
            set;
        }

        [XmlElement("closeprice")]
        public string ClosePrice
        {
            get;
            set;
        }

        [XmlElement("closeyield")]
        public string CloseYield
        {
            get;
            set;
        }

        [XmlElement("status")]
        public string Status
        {
            get;
            set;
        }

        [XmlElement("tradingstatus")]
        public string TradingStatus
        {
            get;
            set;
        }
    }
}