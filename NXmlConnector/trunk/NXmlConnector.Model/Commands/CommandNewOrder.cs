using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandNewOrder : Command
    {
        public NewOrder NewOrder
        {
            get;
            private set;
        }

        public CommandNewOrder(NewOrder newOrder)
            : base("neworder")
        {
            NewOrder = newOrder;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("secid", NewOrder.SecurityId));
            command.Add(new XElement("client", NewOrder.ClientId));
            command.Add(new XElement("price", NewOrder.Price));
            command.Add(new XElement("quantity", NewOrder.Quantity));
            command.Add(new XElement("buysell", NewOrder.OrderType));
            command.Add(new XElement("brokerref", NewOrder.BrokerRef));
            command.Add(new XElement("unfilled", NewOrder.Unfilled));
            if (NewOrder.ByMarket) command.Add(new XElement("bymarket"));
            if (NewOrder.UseCredit) command.Add(new XElement("usecredit"));
            if (NewOrder.NoSplit) command.Add(new XElement("nosplit"));
        }
    }
}
