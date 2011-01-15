#region usings

using System.Runtime.Remoting.Channels;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    public abstract class ServerChannelSinkProviderBase : IServerChannelSinkProvider
    {
        #region Implementation of IServerChannelSinkProvider

        public virtual void GetChannelData(IChannelDataStore channelData)
        {
        }

        public abstract IServerChannelSink CreateSink(IChannelReceiver channel);

        public IServerChannelSinkProvider Next { get; set; }

        #endregion
    }
}