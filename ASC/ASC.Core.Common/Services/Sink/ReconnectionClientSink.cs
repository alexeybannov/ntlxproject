#region usings

using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using ASC.Common.Utils;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Services
{
    internal class ReconnectionClientSink : ClientSinkBase
    {
        public ReconnectionClientSink(IClientChannelSink next)
        {
            SetNextSink(next);
        }

        public override void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream,
                                             object state)
        {
            try
            {
                var error = message as IMethodReturnMessage;
                if (error != null && error.Exception != null &&
                    typeof (RemotingException).Equals(error.Exception.GetType()))
                {
                    CoreContext.Reconnect();
                    LogHolder.Log("ASC.Core.Common").Debug("Reconnection occured.");
                }
            }
            catch (Exception ex)
            {
                LogHolder.Log("ASC.Core.Common").WarnFormat("ProcessResponse error: {0}", ex);
            }
        }
    }
}