#region usings

using System.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Services
{
    internal class ReconnectionClientSinkProvider : ClientChannelSinkProviderBase
    {
        public override IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            return new ReconnectionClientSink(Next.CreateSink(channel, url, remoteChannelData));
        }
    }
}