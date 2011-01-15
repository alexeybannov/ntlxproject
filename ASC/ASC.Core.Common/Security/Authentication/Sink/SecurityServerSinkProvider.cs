#region usings

using System.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Security.Authentication
{
    internal class SecurityServerSinkProvider : ServerChannelSinkProviderBase
    {
        public override IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            var securitySink = new SecurityServerSink();
            if (Next != null)
            {
                IServerChannelSink nextSink = Next.CreateSink(channel);
                if (nextSink != null) securitySink.SetNextSink(nextSink);
            }
            return securitySink;
        }
    }
}