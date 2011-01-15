#region usings

using System.Runtime.Remoting.Channels;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    public abstract class ClientChannelSinkProviderBase : IClientChannelSinkProvider
    {
        #region IClientChannelSinkProvider

        public abstract IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData);

        public IClientChannelSinkProvider Next { get; set; }

        #endregion
    }
}