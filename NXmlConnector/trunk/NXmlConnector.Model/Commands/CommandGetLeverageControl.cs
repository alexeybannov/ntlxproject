using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandGetLeverageControl : Command
    {
        public string Client
        {
            get;
            private set;
        }

        public int[] SecurityIds
        {
            get;
            private set;
        }

        public CommandGetLeverageControl(string client, int[] securityIds)
            : base("get_leverage_control")
        {
            Client = client;
            SecurityIds = securityIds ?? new int[0];
        }

        protected override void WriteElement(XElement command)
        {
            if (!string.IsNullOrEmpty(Client))
            {
                command.Add(new XElement("client", Client));
            }
            foreach (var id in SecurityIds)
            {
                command.Add(new XElement("secid", id));
            }
        }
    }
}
