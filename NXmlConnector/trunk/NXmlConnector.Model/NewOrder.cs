
namespace NXmlConnector.Model
{
    public class NewOrder
    {
        public string ClientId
        {
            get;
            set;
        }

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

        public int Quantity
        {
            get;
            set;
        }

        public OrderType OrderType
        {
            get;
            set;
        }

        public OrderUnfilledType Unfilled
        {
            get;
            set;
        }

        public string BrokerRef
        {
            get;
            set;
        }

        public bool ByMarket
        {
            get;
            set;
        }

        public bool UseCredit
        {
            get;
            set;
        }

        public bool NoSplit
        {
            get;
            set;
        }


        public NewOrder()
        {
            Unfilled = OrderUnfilledType.PutInQueue;
            ByMarket = false;
            UseCredit = false;
            NoSplit = false;
        }

        public NewOrder(int securityId, OrderType orderType, int quantity)
            : this()
        {
            SecurityId = securityId;
            OrderType = orderType;
            Quantity = quantity;
            ByMarket = true;
        }

        public NewOrder(int securityId, OrderType orderType, int quantity, double price)
            : this(securityId, orderType, quantity)
        {
            ByMarket = false;
            Price = price;
        }
    }
}
