using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandSubscribeTicks : Command
    {
        public bool Filter
        {
            get;
            set;
        }


        public CommandSubscribeTicks()
            : base("subscribe_ticks")
        {

        }

        protected override void WriteElement(XElement command)
        {
        }
    }
}
