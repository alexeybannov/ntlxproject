#region usings

using System;
using System.Net;

#endregion

namespace ASC.Net
{
    public sealed class TransportSelector
    {
        public static readonly TransportType[] LocalRangedTransportTypes =
            new[] {TransportType.Ipc, TransportType.Pipe, TransportType.Tcp, TransportType.Http};

        public static readonly TransportType[] RemoteRangedTransportTypes =
            new[] {TransportType.Tcp, TransportType.Http, TransportType.Pipe};

        public static TransportType Select(params TransportType[] types)
        {
            if (types == null || types.Length == 0)
                throw new ArgumentException("types");
            return Select(types, RemoteRangedTransportTypes);
        }

        public static TransportType Select(ConnectionHostEntry Destination, params TransportType[] types)
        {
            if (Destination == null) throw new ArgumentNullException("Destination");
            if (Destination.AddressList == null || Destination.AddressList.Length == 0)
                throw new ArgumentException("Destination");
            if (types == null || types.Length == 0) throw new ArgumentException("types");
            if (
                Dns.GetHostEntry(Destination.HostName).HostName ==
                Dns.GetHostEntry(Dns.GetHostName()).HostName)
                return Select(types, LocalRangedTransportTypes);
            else
                return Select(types, RemoteRangedTransportTypes);
        }

        private static TransportType Select(TransportType[] from, TransportType[] ranged)
        {
            if (from == null || ranged == null) throw new ArgumentNullException();
            foreach (TransportType type in ranged)
            {
                foreach (TransportType fromType in from)
                {
                    if (type == fromType) return fromType;
                }
            }
            throw new NotSupportedException();
        }
    }
}