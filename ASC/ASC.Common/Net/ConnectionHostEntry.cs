#region usings

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

#endregion

namespace ASC.Net
{
    [Serializable]
    [DebuggerDisplay("{Address}")]
    public class ConnectionHostEntry
    {
        #region constructors

        public ConnectionHostEntry(IPAddress[] addressList, int port)
        {
            if (addressList == null) throw new ArgumentNullException("addressList");
            if ((port < 0) || (port > 0xffff)) throw new ArgumentOutOfRangeException("port");
            if (0 < addressList.Length) HostName = Dns.GetHostEntry(addressList[0]).HostName;
            Port = port;
        }

        public ConnectionHostEntry(string hostName, int port)
        {
            if (hostName == null) throw new ArgumentNullException("hostName");
            if ((port < 0) || (port > 0xffff)) throw new ArgumentOutOfRangeException("port");
            HostName = hostName;
            Port = port;
        }

        public ConnectionHostEntry(string address)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (address.IndexOf(":") < 0) throw new ArgumentException("address");
            HostName = address.Split(':')[0];
            Port = Convert.ToInt32(address.Split(':')[1]);
            if ((Port < 0) || (Port > 0xffff)) throw new ArgumentOutOfRangeException("port");
        }

        #endregion

        #region properties

        public IPAddress[] AddressList
        {
            get
            {
                return Array.FindAll(Dns.GetHostEntry(HostName).AddressList,
                                     (ad) => ad.AddressFamily == AddressFamily.InterNetwork);
            }
        }

        public int Port { get; set; }

        public string IpcPort
        {
            get { return "localhost:" + Port; }
        }

        public string HostName { get; private set; }

        public bool IsLocal
        {
            get
            {
                return String.Equals(
                    Dns.GetHostEntry(Dns.GetHostName()).HostName,
                    Dns.GetHostEntry(HostName).HostName
                    );
            }
        }

        public string Address
        {
            get { return String.Format("{0}:{1}", HostName, Port); }
        }

        #endregion

        public static ConnectionHostEntry GetLocalhost(int port)
        {
            return new ConnectionHostEntry(Dns.GetHostName(), port);
        }

        public override bool Equals(object obj)
        {
            var hostEntry = obj as ConnectionHostEntry;
            if (hostEntry == null) return false;
            return
                String.Equals(Dns.GetHostEntry(hostEntry.HostName).HostName, Dns.GetHostEntry(HostName).HostName) &&
                Port.Equals(hostEntry.Port);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override string ToString()
        {
            return Address;
        }
    }
}