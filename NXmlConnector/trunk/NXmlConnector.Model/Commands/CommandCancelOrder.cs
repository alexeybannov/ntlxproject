using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandCancelOrder : Command
    {
        public int TransactionId
        {
            get;
            private set;
        }

        public CommandCancelOrder(int transactionId)
            : base("cancelorder")
        {
            TransactionId = transactionId;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("transactionid", TransactionId));
        }
    }
}
