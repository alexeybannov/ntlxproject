using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandConnect : Command
    {
        public string Login
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string LogsDirectory
        {
            get;
            set;
        }

        public LogLevel LogLevel
        {
            get;
            set;
        }

        public bool AutoPos
        {
            get;
            set;
        }

        public string NotesFileName
        {
            get;
            set;
        }

        public Proxy Proxy
        {
            get;
            set;
        }


        public CommandConnect()
            : base("connect")
        {

        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("login", Login));
            command.Add(new XElement("password", Password));
            command.Add(new XElement("host", Host));
            command.Add(new XElement("port", Port));
            command.Add(new XElement("autopos", AutoPos));
            command.Add(new XElement("logsdir", LogsDirectory));
            if (LogLevel != Model.LogLevel.None) command.Add(new XElement("loglevel", LogLevel.ToString("d")));
            if (NotesFileName != null) command.Add(new XElement("notes_file", NotesFileName));
            if (Proxy != null)
            {
                command.Add(new XElement("proxy",
                    new XAttribute("type", Proxy.ProxyType == ProxyType.HttpConnect ? "HTTP-CONNECT" : Proxy.ProxyType.ToString().ToUpper()),
                    new XAttribute("addr", Proxy.Host),
                    new XAttribute("port", Proxy.Port),
                    new XAttribute("login", Proxy.Login),
                    new XAttribute("password", Proxy.Password)));
            }
        }
    }
}
