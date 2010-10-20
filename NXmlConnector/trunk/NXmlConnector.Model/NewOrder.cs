
namespace NXmlConnector.Model
{
    public class NewOrder
    {
        public string SecurityId
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

        public NewOrder(string securityId, OrderType orderType)
            : this()
        {
            SecurityId = securityId;
            OrderType = orderType;
        }
    }
}
