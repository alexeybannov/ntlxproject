using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    public class Proxy
    {
        public ProxyType ProxyType
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


        public Proxy()
        {
        }

        public Proxy(ProxyType proxyType, string host, int port, string login, string passwoed)
        {
            ProxyType = proxyType;
            Host = host;
            Port = port;
            Login = login;
            Password = passwoed;
        }
    }
}
