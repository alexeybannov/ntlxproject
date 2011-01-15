#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Tenants.Sink
{
    internal class TenantClientSink : ClientSinkBase
    {
        public TenantClientSink(IClientChannelSink next)
        {
            SetNextSink(next);
        }

        public override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream,
                                            ref object state)
        {
            ClientTenantManager.TenantThrow();
        }
    }
}