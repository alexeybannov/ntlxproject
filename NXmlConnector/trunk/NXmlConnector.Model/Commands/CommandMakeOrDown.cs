using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandMakeOrDown : Command
    {
        public int TransactionId
        {
            get;
            private set;
        }

        public CommandMakeOrDown(int transactionId)
            : base("makeordown")
        {
            TransactionId = transactionId;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("transactionid", TransactionId));
        }
    }
}
