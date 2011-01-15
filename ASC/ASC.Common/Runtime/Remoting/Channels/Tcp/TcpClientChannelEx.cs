#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging;
using ASC.Common.Utils;

#endregion

namespace ASC.Runtime.Remoting.Channels.Tcp
{
    public class TcpClientChannelEx : TcpClientChannel, IChannelWithServices
    {
        public TcpClientChannelEx(IDictionary properties, IClientChannelSinkProvider provider)
            : base(properties, provider)
        {
            Services = new List<Guid>();
        }

        #region IChannelWithServices Members

        public IList<Guid> Services { get; private set; }

        public void AttachService(Guid serviceID)
        {
            if (!Services.Contains(serviceID)) Services.Add(serviceID);
        }

        public void DeattachService(Guid serviceID)
        {
            Services.Remove(serviceID);
        }

        #endregion

        public override IMessageSink CreateMessageSink(string url, object remoteChannelData, out string objectURI)
        {
            objectURI = null;
            if (!ChannelSelector.IsClientChannelValid(this, remoteChannelData as RemoteChannelData, remoteChannelData))
            {
                return null;
            }
            IMessageSink msgSink = base.CreateMessageSink(url, remoteChannelData, out objectURI);
            LogHolder.Log("ASC.Common").DebugFormat("[CreateMessageSink] using {0} for {1}", ChannelName, objectURI);
            return msgSink;
        }
    }
}