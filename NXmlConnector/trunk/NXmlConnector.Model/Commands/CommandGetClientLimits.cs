using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandGetClientLimits : Command
    {
        public string Client
        {
            get;
            private set;
        }

        public CommandGetClientLimits(string client)
            : base("get_client_limits")
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
