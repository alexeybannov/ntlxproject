#region usings

using System.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Common.Remoting
{
    internal class DebugServerSinkProvider : ServerChannelSinkProviderBase
    {
        public override IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            var debugSink = new DebugServerSink();
            if (Next != null)
            {
                IServerChannelSink nextSink = Next.CreateSink(channel);
                if (nextSink != null) debugSink.SetNextSink(nextSink);
            }
            return debugSink;
        }
    }
}