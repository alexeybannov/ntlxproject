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

        public string Address
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

        public Proxy(ProxyType proxyType, string addrss, int port, string login, string passwoed)
        {
            ProxyType = proxyType;
            Address = addrss;
            Port = port;
            Login = login;
            Password = passwoed;
        }
    }
}
