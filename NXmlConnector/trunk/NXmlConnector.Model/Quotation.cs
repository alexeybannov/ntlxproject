using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quotation
    {
        public int SecurityId
        {
            get;
            set;
        }

        public double AccruedIntValue
        {
            get;
            set;
        }

        public double Open
        {
            get;
            set;
        }

        public double Waprice
        {
            get;
            set;
        }

        public int BidDeptht
        {
            get;
            set;
        }

        public int NumBids
        {
            get;
            set;
        }

        public int OfferDeptht
        {
            get;
            set;
        }

        public double Bid
        {
            get;
            set;
        }

        public double Offer
        {
            get;
            set;
        }

        public int NumOffers
        {
            get;
            set;
        }

        public int NumTrades
        {
            get;
            set;
        }

        public int VolToday
        {
            get;
            set;
        }

        public int OpenPositions
        {
            get;
            set;
        }

        public int DeltaPositions
        {
            get;
            set;
        }

        public double Last
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public TimeSpan Time
        {
            get;
            set;
        }

        public double Change
        {
            get;
            set;
        }

        public double PriceMinusPrevWaprice
        {
            get;
            set;
        }

        public double ValToDay
        {
            get;
            set;
        }

        public double Yield
        {
            get;
            set;
        }

        public double YielDatWaprice
        {
            get;
            set;
        }

        public double MarketPriceToday
        {
            get;
            set;
        }

        public double HighBid
        {
            get;
            set;
        }

        public double LowOffer
        {
            get;
            set;
        }

        public double High
        {
            get;
            set;
        }

        public double Low
        {
            get;
            set;
        }

        public double ClosePrice
        {
            get;
            set;
        }

        public double CloseYield
        {
            get;
            set;
        }

        public QuotationStatus Status
        {
            get;
            set;
        }

        public TradingStatus TradingStatus
        {
            get;
            set;
        }

        [XmlIgnore]
        public QuotationChanges LastChanges
        {
            get;
            private set;
        }


        internal void Update(_Quotation q)
        {
            LastChanges = new QuotationChanges();
            
            LastChanges.AccruedIntValueChanged = q.AccruedIntValue.HasValue;
            if (q.AccruedIntValue.HasValue) AccruedIntValue = q.AccruedIntValue.Value;
            
            LastChanges.BidChanged = q.Bid.HasValue;
            if (q.Bid.HasValue) Bid = q.Bid.Value;
            
            LastChanges.BidDepthtChanged = q.BidDeptht.HasValue;
            if (q.BidDeptht.HasValue) BidDeptht = q.BidDeptht.Value;

            LastChanges.ChangeChanged = q.Change.HasValue;
            if (q.Change.HasValue) Change = q.Change.Value;

            LastChanges.ClosePriceChanged = q.ClosePrice.HasValue;
            if (q.ClosePrice.HasValue) ClosePrice = q.ClosePrice.Value;

            LastChanges.CloseYieldChanged = q.CloseYield.HasValue;
            if (q.CloseYield.HasValue) CloseYield = q.CloseYield.Value;

            LastChanges.DeltaPositionsChanged = q.DeltaPositions.HasValue;
            if (q.DeltaPositions.HasValue) DeltaPositions = q.DeltaPositions.Value;

            LastChanges.HighBidChanged = q.HighBid.HasValue;
            if (q.HighBid.HasValue) HighBid = q.HighBid.Value;

            LastChanges.HighChanged = q.High.HasValue;
            if (q.High.HasValue) High = q.High.Value;

            LastChanges.LastChanged = q.Last.HasValue;
            if (q.Last.HasValue) Last = q.Last.Value;

            LastChanges.LowChanged = q.Low.HasValue;
            if (q.Low.HasValue) Low = q.Low.Value;

            LastChanges.LowOfferChanged = q.LowOffer.HasValue;
            if (q.LowOffer.HasValue) LowOffer = q.LowOffer.Value;

            LastChanges.MarketPriceTodayChanged = q.MarketPriceToday.HasValue;
            if (q.MarketPriceToday.HasValue) MarketPriceToday = q.MarketPriceToday.Value;

            LastChanges.NumBidsChanged = q.NumBids.HasValue;
            if (q.NumBids.HasValue) NumBids = q.NumBids.Value;

            LastChanges.NumOffersChanged = q.NumOffers.HasValue;
            if (q.NumOffers.HasValue) NumOffers = q.NumOffers.Value;

            LastChanges.NumTradesChanged = q.NumTrades.HasValue;
            if (q.NumTrades.HasValue) NumTrades = q.NumTrades.Value;

            LastChanges.OfferChanged = q.Offer.HasValue;
            if (q.Offer.HasValue) Offer = q.Offer.Value;

            LastChanges.OfferDepthtChanged = q.OfferDeptht.HasValue;
            if (q.OfferDeptht.HasValue) OfferDeptht = q.OfferDeptht.Value;

            LastChanges.OpenChanged = q.Open.HasValue;
            if (q.Open.HasValue) Open = q.Open.Value;

            LastChanges.OpenPositionsChanged = q.OpenPositions.HasValue;
            if (q.OpenPositions.HasValue) OpenPositions = q.OpenPositions.Value;

            LastChanges.PriceMinusPrevWapriceChanged = q.PriceMinusPrevWaprice.HasValue;
            if (q.PriceMinusPrevWaprice.HasValue) PriceMinusPrevWaprice = q.PriceMinusPrevWaprice.Value;

            LastChanges.QuantityChanged = q.Quantity.HasValue;
            if (q.Quantity.HasValue) Quantity = q.Quantity.Value;

            LastChanges.StatusChanged = q.Status.HasValue;
            if (q.Status.HasValue) Status = q.Status.Value;

            LastChanges.TimeChanged = !string.IsNullOrEmpty(q.Time);
            if (!string.IsNullOrEmpty(q.Time)) Time = TimeSpan.Parse(q.Time);

            LastChanges.TradingStatusChanged = q.TradingStatus.HasValue;
            if (q.TradingStatus.HasValue) TradingStatus = q.TradingStatus.Value;

            LastChanges.ValToDayChanged = q.ValToDay.HasValue;
            if (q.ValToDay.HasValue) ValToDay = q.ValToDay.Value;

            LastChanges.VolTodayChanged = q.VolToday.HasValue;
            if (q.VolToday.HasValue) VolToday = q.VolToday.Value;

            LastChanges.WapriceChanged = q.Waprice.HasValue;
            if (q.Waprice.HasValue) Waprice = q.Waprice.Value;

            LastChanges.YielDatWapriceChanged = q.YielDatWaprice.HasValue;
            if (q.YielDatWaprice.HasValue) YielDatWaprice = q.YielDatWaprice.Value;

            LastChanges.YieldChanged = q.Yield.HasValue;
            if (q.Yield.HasValue) Yield = q.Yield.Value;
        }
    }


    public class _Quotation
    {
        [XmlAttribute("secid")]
        public int SecurityId;

        [XmlElement("accruedintvalue")]
        public double? AccruedIntValue;

        [XmlElement("open")]
        public double? Open;

        [XmlElement("waprice")]
        public double? Waprice;

        [XmlElement("biddeptht")]
        public int? BidDeptht;

        [XmlElement("numbids")]
        public int? NumBids;

        [XmlElement("offerdeptht")]
        public int? OfferDeptht;

        [XmlElement("bid")]
        public double? Bid;

        [XmlElement("offer")]
        public double? Offer;

        [XmlElement("numoffers")]
        public int? NumOffers;

        [XmlElement("numtrades")]
        public int? NumTrades;

        [XmlElement("voltoday")]
        public int? VolToday;

        [XmlElement("openpositions")]
        public int? OpenPositions;

        [XmlElement("deltapositions")]
        public int? DeltaPositions;

        [XmlElement("last")]
        public double? Last;

        [XmlElement("quantity")]
        public int? Quantity;

        [XmlElement("time")]
        public string Time;

        [XmlElement("change")]
        public double? Change;

        [XmlElement("priceminusprevwaprice")]
        public double? PriceMinusPrevWaprice;

        [XmlElement("valtoday")]
        public double? ValToDay;

        [XmlElement("yield")]
        public double? Yield;

        [XmlElement("yieldatwaprice")]
        public double? YielDatWaprice;

        [XmlElement("marketpricetoday")]
        public double? MarketPriceToday;

        [XmlElement("highbid")]
        public double? HighBid;

        [XmlElement("lowoffer")]
        public double? LowOffer;

        [XmlElement("high")]
        public double? High;

        [XmlElement("low")]
        public double? Low;

        [XmlElement("closeprice")]
        public double? ClosePrice;

        [XmlElement("closeyield")]
        public double? CloseYield;

        [XmlElement("status")]
        public QuotationStatus? Status;

        [XmlElement("tradingstatus")]
        public TradingStatus? TradingStatus;
    }

    public class QuotationChanges
    {
        public bool AccruedIntValueChanged
        {
            get;
            set;
        }

        public bool OpenChanged
        {
            get;
            set;
        }

        public bool WapriceChanged
        {
            get;
            set;
        }

        public bool BidDepthtChanged
        {
            get;
            set;
        }

        public bool NumBidsChanged
        {
            get;
            set;
        }

        public bool OfferDepthtChanged
        {
            get;
            set;
        }

        public bool BidChanged
        {
            get;
            set;
        }

        public bool OfferChanged
        {
            get;
            set;
        }

        public bool NumOffersChanged
        {
            get;
            set;
        }

        public bool NumTradesChanged
        {
            get;
            set;
        }

        public bool VolTodayChanged
        {
            get;
            set;
        }

        public bool OpenPositionsChanged
        {
            get;
            set;
        }

        public bool DeltaPositionsChanged
        {
            get;
            set;
        }

        public bool LastChanged
        {
            get;
            set;
        }

        public bool QuantityChanged
        {
            get;
            set;
        }

        public bool TimeChanged
        {
            get;
            set;
        }

        public bool ChangeChanged
        {
            get;
            set;
        }

        public bool PriceMinusPrevWapriceChanged
        {
            get;
            set;
        }

        public bool ValToDayChanged
        {
            get;
            set;
        }

        public bool YieldChanged
        {
            get;
            set;
        }

        public bool YielDatWapriceChanged
        {
            get;
            set;
        }

        public bool MarketPriceTodayChanged
        {
            get;
            set;
        }

        public bool HighBidChanged
        {
            get;
            set;
        }

        public bool LowOfferChanged
        {
            get;
            set;
        }

        public bool HighChanged
        {
            get;
            set;
        }

        public bool LowChanged
        {
            get;
            set;
        }

        public bool ClosePriceChanged
        {
            get;
            set;
        }

        public bool CloseYieldChanged
        {
            get;
            set;
        }

        public bool StatusChanged
        {
            get;
            set;
        }

        public bool TradingStatusChanged
        {
            get;
            set;
        }
    }
}