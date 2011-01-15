#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using ASC.Common.Security.Authentication;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Security.Authentication
{
    internal class SecurityClientSink : ClientSinkBase
    {
        public SecurityClientSink(IClientChannelSink next)
        {
            SetNextSink(next);
        }

        public override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream,
                                            ref object state)
        {
            if (AuthenticationContext.Principal.Identity is IAccount)
            {
                message.Properties["CURRENT_PRINCIPAL"] = AuthenticationContext.Principal;
            }
        }
    }
}