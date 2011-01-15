#region usings

using System.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Security.Authentication
{
    internal class SecurityClientSinkProvider : ClientChannelSinkProviderBase
    {
        public override IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            return new SecurityClientSink(Next.CreateSink(channel, url, remoteChannelData));
        }
    }
}