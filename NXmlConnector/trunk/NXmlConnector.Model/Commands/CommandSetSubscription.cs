using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandSetSubscription : Command
    {
        public int[] AllTrades
        {
            get;
            set;
        }

        public int[] Quotations
        {
            get;
            set;
        }

        public int[] Quotes
        {
            get;
            set;
        }


        public CommandSetSubscription()
            : base("subscribe")
        {
        }


        public static CommandSetSubscription Subscribe(IEnumerable<int> ids, Subscription to)
        {
            var command = new CommandSetSubscription();
            if ((to & Subscription.Trades) == Subscription.Trades) command.AllTrades = ids.ToArray();
            if ((to & Subscription.Quotations) == Subscription.Quotations) command.Quotations = ids.ToArray();
            if ((to & Subscription.Quotes) == Subscription.Quotes) command.Quotes = ids.ToArray();
            return command;
        }

        public static CommandSetSubscription Unsubscribe(IEnumerable<int> ids, Subscription to)
        {
            var command = Subscribe(ids, to);
            command.Id = "unsubscribe";
            return command;
        }

        protected override void WriteElement(XElement command)
        {
            if (AllTrades != null && 0 < AllTrades.Count())
            {
                var trades = new XElement("alltrades");
                command.Add(trades);
                foreach (var id in AllTrades)
                {
                    trades.Add(new XElement("secid", id));
                }
            }

            if (Quotations != null && 0 < Quotations.Count())
            {
                var quotations = new XElement("quotations");
                command.Add(quotations);
                foreach (var id in Quotations)
                {
                    quotations.Add(new XElement("secid", id));
                }
            }

            if (Quotes != null && 0 < Quotes.Count())
            {
                var quotes = new XElement("quotes");
                command.Add(quotes);
                foreach (var id in Quotes)
                {
                    quotes.Add(new XElement("secid", id));
                }
            }
        }
    }
}
