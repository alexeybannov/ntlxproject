using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using ASC.Common.Utils;
using ASC.Runtime.Remoting.Sinks;

namespace ASC.Core.Common.Remoting
{
    internal class DebugServerSink : ServerSinkBase
    {
        public override void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
        {
            var method = message as IMethodCallMessage;
            if (method != null)
            {
                LogHolder.Log("ASC.Core.Common").DebugFormat("Remoting call: {0}", GetSignature(method));
            }
        }

        public override void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
        {
            try
            {
                var method = message as IMethodReturnMessage;
                if (method != null && method.Exception != null)
                {
                    LogHolder.Log("ASC.Core.Common").WarnFormat("Exception on call {0}:\r\n{1}", GetSignature(method), method.Exception);
                }
            }
            catch (Exception ex)
            {
                LogHolder.Log("ASC.Core.Common").WarnFormat("ProcessResponse error: {0}", ex);
            }
        }


        private string GetSignature(IMethodMessage method)
        {
            return string.Format("{0}.{1}({2}) by {3}",
                method.TypeName.Remove(method.TypeName.IndexOf(',')),
                method.MethodName,
                string.Join(", ", method.Args.Select(a => a != null ? a.ToString() : "null").ToArray()),
                Thread.CurrentPrincipal.Identity.IsAuthenticated ? Thread.CurrentPrincipal.Identity.Name : "Anonymous");
        }
    }
}