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


        public static CommandSetSubscription Subscribe(IEnumerable<int> trades, IEnumerable<int> quotations, IEnumerable<int> quotes)
        {
            var command = new CommandSetSubscription();
            if (trades != null) command.AllTrades = trades.ToArray();
            if (quotations != null) command.Quotations = quotations.ToArray();
            if (quotes != null) command.Quotes = quotes.ToArray();
            return command;
        }

        public static CommandSetSubscription Unsubscribe(IEnumerable<int> trades, IEnumerable<int> quotations, IEnumerable<int> quotes)
        {
            var command = Subscribe(trades, quotations, quotes);
            command.Id = "unsubscribe";
            return command;
        }

        protected override void WriteElement(XElement command)
        {
            if (AllTrades != null)
            {
                var trades = new XElement("alltrades");
                command.Add(trades);
                foreach (var id in AllTrades)
                {
                    trades.Add(new XElement("secid", id));
                }
            }

            if (Quotations != null)
            {
                var quotations = new XElement("quotations");
                command.Add(quotations);
                foreach (var id in Quotations)
                {
                    quotations.Add(new XElement("secid", id));
                }
            }

            if (Quotes != null)
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
