#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

#endregion

namespace ASC.Runtime.Remoting.Channels.Tcp
{
    [DebuggerDisplay("Name = {ChannelName}, Type = {Type}")]
    public class TcpServerChannelEx : TcpServerChannel, IChannelWithServices
    {
        public TcpServerChannelEx(IDictionary properties, IServerChannelSinkProvider sinkProvider) :
            base(properties, sinkProvider)
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
    }
}