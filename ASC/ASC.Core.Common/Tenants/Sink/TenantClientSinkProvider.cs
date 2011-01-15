#region usings

using System.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Tenants.Sink
{
    internal class TenantClientSinkProvider : ClientChannelSinkProviderBase
    {
        public override IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            return new TenantClientSink(Next.CreateSink(channel, url, remoteChannelData));
        }
    }
}