using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandGetFortsPosition : Command
    {
        public string Client
        {
            get;
            private set;
        }

        public CommandGetFortsPosition(string client)
            : base("get_forts_positions")
        {
            Client = client;
        }

        protected override void WriteElement(XElement command)
        {
            if (!string.IsNullOrEmpty(Client))
            {
                command.Add(new XElement("client", Client));
            }
        }
    }
}
