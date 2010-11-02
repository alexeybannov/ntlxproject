using System;
using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class AllTrade
    {
        public int SecurityId
        {
            get;
            set;
        }

        public int TradeNo
        {
            get;
            set;
        }

        public TimeSpan Time
        {
            get;
            set;
        }

        public string Board
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public OrderType BuySell
        {
            get;
            set;
        }

        public TradingStatus Period
        {
            get;
            set;
        }

        public string OpenInterest
        {
            get;
            set;
        }

        [XmlIgnore]
        public AllTradeChanges LastChanges
        {
            get;
            private set;
        }


        internal void Update(_AllTrade t)
        {
            LastChanges = new AllTradeChanges();

            LastChanges.BoardChanged = !string.IsNullOrEmpty(t.Board);
            if (!string.IsNullOrEmpty(t.Board)) Board = t.Board;

            LastChanges.BuySellChanged = t.BuySell.HasValue;
            if (t.BuySell.HasValue) BuySell = t.BuySell.Value;

            LastChanges.OpenInterestChanged = !string.IsNullOrEmpty(t.OpenInterest);
            if (!string.IsNullOrEmpty(t.OpenInterest)) OpenInterest = t.OpenInterest;

            LastChanges.PeriodChanged = t.Period.HasValue;
            if (t.Period.HasValue) Period = t.Period.Value;

            LastChanges.PriceChanged = t.Price.HasValue;
            if (t.Price.HasValue) Price = t.Price.Value;

            LastChanges.QuantityChanged = t.Quantity.HasValue;
            if (t.Quantity.HasValue) Quantity = t.Quantity.Value;

            LastChanges.TimeChanged = !string.IsNullOrEmpty(t.Time);
            if (!string.IsNullOrEmpty(t.Time)) Time = TimeSpan.Parse(t.Time);

            LastChanges.TradeNoChanged = t.TradeNo.HasValue;
            if (t.TradeNo.HasValue) TradeNo = t.TradeNo.Value;
        }
    }


    public class AllTradeChanges
    {
        public bool TradeNoChanged
        {
            get;
            set;
        }

        public bool TimeChanged
        {
            get;
            set;
        }

        public bool BoardChanged
        {
            get;
            set;
        }

        public bool PriceChanged
        {
            get;
            set;
        }

        public bool QuantityChanged
        {
            get;
            set;
        }

        public bool BuySellChanged
        {
            get;
            set;
        }

        public bool PeriodChanged
        {
            get;
            set;
        }

        public bool OpenInterestChanged
        {
            get;
            set;
        }
    }

    public class _AllTrade
    {
        [XmlAttribute("secid")]
        public int SecurityId;

        [XmlElement("tradeno")]
        public int? TradeNo;

        [XmlElement("time")]
        public string Time;

        [XmlElement("board")]
        public string Board;

        [XmlElement("price")]
        public double? Price;

        [XmlElement("quantity")]
        public int? Quantity;

        [XmlElement("buysell")]
        public OrderType? BuySell;

        [XmlElement("period")]
        public TradingStatus? Period;

        [XmlElement("openinterest")]
        public string OpenInterest;
    }
}