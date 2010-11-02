using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Quote
    {
        public int SecurityId
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }

        public double Yield
        {
            get;
            set;
        }

        public int Buy
        {
            get;
            set;
        }

        public int Sell
        {
            get;
            set;
        }

        [XmlIgnore]
        public QuoteChanges LastChanges
        {
            get;
            private set;
        }

        internal void Update(_Quote q)
        {
            LastChanges = new QuoteChanges();

            LastChanges.BuyChanged = q.Buy.HasValue;
            if (q.Buy.HasValue) Buy = q.Buy.Value;

            LastChanges.PriceChanged = q.Price.HasValue;
            if (q.Price.HasValue) Price = q.Price.Value;

            LastChanges.SellChanged = q.Sell.HasValue;
            if (q.Sell.HasValue) Sell = q.Sell.Value;

            LastChanges.YieldChanged = q.Yield.HasValue;
            if (q.Yield.HasValue) Yield = q.Yield.Value;
        }
    }

    public class QuoteChanges
    {
        public bool PriceChanged
        {
            get;
            set;
        }

        public bool YieldChanged
        {
            get;
            set;
        }

        public bool BuyChanged
        {
            get;
            set;
        }

        public bool SellChanged
        {
            get;
            set;
        }
    }

    public class _Quote
    {
        [XmlAttribute("secid")]
        public int SecurityId;

        [XmlElement("price")]
        public double? Price;

        [XmlElement("yield")]
        public double? Yield;

        [XmlElement("buy")]
        public int? Buy;

        [XmlElement("sell")]
        public int? Sell;
    }
}