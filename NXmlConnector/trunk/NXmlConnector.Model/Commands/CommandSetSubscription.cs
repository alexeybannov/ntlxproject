using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandSetSubscription : Command
    {
        public string[] AllTrades
        {
            get;
            set;
        }

        public string[] Quotations
        {
            get;
            set;
        }

        public string[] Quotes
        {
            get;
            set;
        }


        public CommandSetSubscription()
            : base("subscribe")
        {
        }


        public static CommandSetSubscription Subscribe(IEnumerable<string> trades, IEnumerable<string> quotations, IEnumerable<string> quotes)
        {
            var command = new CommandSetSubscription();
            if (trades != null) command.AllTrades = trades.ToArray();
            if (quotations != null) command.Quotations = quotations.ToArray();
            if (quotes != null) command.Quotes = quotes.ToArray();
            return command;
        }

        public static CommandSetSubscription Unsubscribe(IEnumerable<string> trades, IEnumerable<string> quotations, IEnumerable<string> quotes)
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
                foreach (var secid in AllTrades)
                {
                    trades.Add(new XElement("secid", secid));
                }
            }

            if (Quotations != null)
            {
                var quotations = new XElement("quotations");
                command.Add(quotations);
                foreach (var secid in Quotations)
                {
                    quotations.Add(new XElement("secid", secid));
                }
            }

            if (Quotes != null)
            {
                var quotes = new XElement("quotes");
                command.Add(quotes);
                foreach (var secid in Quotes)
                {
                    quotes.Add(new XElement("secid", secid));
                }
            }
        }
    }
}
