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

        public int[] SecurityIds
        {
            get;
            set;
        }

        public int[] TradeNos
        {
            get;
            set;
        }

        public CommandSubscribeTicks()
            : base("subscribe_ticks")
        {
            Filter = false;
        }

        public CommandSubscribeTicks(int[] securityIds, int[] tradeNos, bool filter)
            : this()
        {
            SecurityIds = securityIds;
            TradeNos = tradeNos;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XAttribute("filter", Filter));
            if (SecurityIds != null)
            {
                for (int i = 0; i < SecurityIds.Length; i++)
                {
                    command.Add(new XElement("security",
                        new XAttribute("secid", SecurityIds[i]),
                        new XAttribute("tradeno", TradeNos[i])));
                }
            }
        }
    }
}
