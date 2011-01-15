#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

#endregion

namespace ASC.Runtime.Remoting.Channels.Http
{
    [DebuggerDisplay("Name = {ChannelName}, Type = {Type}")]
    public class HttpServerChannelEx : HttpServerChannel, IChannelWithServices
    {
        public HttpServerChannelEx(IDictionary properties, IServerChannelSinkProvider sinkProvider)
            :
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