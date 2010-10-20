using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandGetHistoryData : Command
    {
        public string SecurityId
        {
            get;
            set;
        }

        public int Period
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public bool Reset
        {
            get;
            set;
        }


        public CommandGetHistoryData()
            : base("gethistorydata")
        {

        }

        protected override void WriteElement(XElement command)
        {
            command.Add(
                new XAttribute("secid", SecurityId),
                new XAttribute("period", Period),
                new XAttribute("count", Count),
                new XAttribute("reset", Reset));
        }
    }
}
