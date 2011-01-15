#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using ASC.Core.Configuration;
using ASC.Runtime.Remoting.Sinks;

#endregion

namespace ASC.Core.Security.Authentication
{
    internal class SecurityServerSink : ServerSinkBase
    {
        public override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream,
                                            ref object state)
        {
            if (message.Properties.Contains("CURRENT_PRINCIPAL"))
            {
                AuthenticationContext.Principal = message.Properties["CURRENT_PRINCIPAL"] as IPrincipal ??
                                                  Constants.Anonymus;
            }
        }
    }
}