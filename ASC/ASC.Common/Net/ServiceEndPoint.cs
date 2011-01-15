#region usings

using System;
using System.Diagnostics;
using System.Text;

#endregion

namespace ASC.Net
{
    [Serializable]
    [DebuggerDisplay("{ServiceUri}")]
    public class ServiceEndPoint
    {
        private string _CachedServiceUri;

        public ServiceEndPoint(ConnectionHostEntry hostEntry, TransportType transportType, string uri)
        {
            if (null == hostEntry) throw new ArgumentNullException("hostEntry");
            Uri = uri;
            TransportType = transportType;
            ConnectionHostEntry = hostEntry;
        }

        #region properties

        public ConnectionHostEntry ConnectionHostEntry { get; private set; }
        public string Uri { get; private set; }
        public TransportType TransportType { get; private set; }

        public string ServiceUri
        {
            get
            {
                if (_CachedServiceUri == null)
                {
                    var sb = new StringBuilder(20);
                    switch (TransportType)
                    {
                        case TransportType.Tcp:
                            sb.Append(@"tcp://");
                            break;
                        case TransportType.Pipe:
                            throw new NotSupportedException();
                        case TransportType.Ipc:
                            sb.Append(@"ipc://");
                            break;
                        case TransportType.Http:
                            sb.Append(@"http://");
                            break;
                        case TransportType.Udp:
                            throw new NotSupportedException();
                    }
                    if (TransportType.Ipc == TransportType) sb.AppendFormat(@"{0}/", ConnectionHostEntry.IpcPort);
                    else sb.AppendFormat(@"{0}:{1}/", ConnectionHostEntry.AddressList[0], ConnectionHostEntry.Port);
                    sb.Append(Uri);
                    _CachedServiceUri = sb.ToString();
                }
                return _CachedServiceUri;
            }
        }

        #endregion
    }
}