using System;

namespace NXmlConnector.Model
{
    public class NewCondOrder : NewOrder
    {
        public OrderCondition ConditionType
        {
            get;
            set;
        }

        public double ConditionValue
        {
            get;
            set;
        }

        public DateTime? ValidAfter
        {
            get;
            set;
        }

        public DateTime? ValidBefore
        {
            get;
            set;
        }

        public bool TillCanceled
        {
            get;
            set;
        }


        public NewCondOrder()
            : base()
        {
        }

        public NewCondOrder(int securityId, OrderType orderType, int quantity)
            : base(securityId, orderType, quantity)
        {
        }

        public NewCondOrder(int securityId, OrderType orderType, int quantity, double price)
            : base(securityId, orderType, quantity, price)
        {
        }
    }
}
