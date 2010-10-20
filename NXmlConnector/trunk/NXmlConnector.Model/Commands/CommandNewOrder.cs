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

        public string ClientId
        {
            get;
            private set;
        }

        public CommandNewOrder(NewOrder newOrder, string clientId)
            : base("neworder")
        {
            NewOrder = newOrder;
            ClientId = clientId;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("secid", NewOrder.SecurityId));
            command.Add(new XElement("client", ClientId));
            command.Add(new XElement("price", NewOrder.Price));
            command.Add(new XElement("quantity", NewOrder.Quantity));
            command.Add(new XElement("buysell", NXmlConverter.ToString(NewOrder.OrderType)));
            command.Add(new XElement("brokerref", NewOrder.BrokerRef));
            command.Add(new XElement("unfilled", NewOrder.Unfilled));
            if (NewOrder.ByMarket) command.Add(new XElement("bymarket"));
            if (NewOrder.UseCredit) command.Add(new XElement("usecredit"));
            if (NewOrder.NoSplit) command.Add(new XElement("nosplit"));
        }
    }
}
