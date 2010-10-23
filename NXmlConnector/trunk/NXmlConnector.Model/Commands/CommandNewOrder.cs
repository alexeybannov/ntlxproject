using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandNewOrder : Command
    {
        public NewOrder Order
        {
            get;
            private set;
        }

        public CommandNewOrder(NewOrder newOrder)
            : base("neworder")
        {
            Order = newOrder;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("secid", Order.SecurityId));
            command.Add(new XElement("client", Order.ClientId));
            command.Add(new XElement("price", Order.Price));
            command.Add(new XElement("quantity", Order.Quantity));
            command.Add(new XElement("buysell", Order.OrderType));
            command.Add(new XElement("brokerref", Order.BrokerRef));
            if (Order.ByMarket) command.Add(new XElement("bymarket"));
            if (Order.UseCredit) command.Add(new XElement("usecredit"));
            if (Order.NoSplit) command.Add(new XElement("nosplit"));

            if (Order.GetType() == typeof(NewOrder))
            {
                command.Add(new XElement("unfilled", Order.Unfilled));
            }
            if (Order.GetType() == typeof(NewCondOrder))
            {
                var condOrder = (NewCondOrder)Order;
                command.SetAttributeValue("id", "newcondorder");
                command.Add(new XElement("cond_type", condOrder.ConditionType));
                command.Add(new XElement("cond_value", condOrder.ConditionValue));
                command.Add(new XElement("validafter", NXmlConverter.ToString(condOrder.ValidAfter)));
                command.Add(new XElement("validbefore", NXmlConverter.ToString(condOrder.ValidBefore)));
                if (condOrder.TillCanceled) command.SetElementValue("validbefore", "till_canceled");
            }
        }
    }
}
