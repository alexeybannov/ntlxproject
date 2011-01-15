#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    public abstract class ClientSinkBase : SinkBase, IClientChannelSink
    {
        #region IClientChannelSink Members

        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream,
                                   out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            object state = null;
            ProcessRequest(msg, requestHeaders, ref requestStream, ref state);
            NextChannelSink.ProcessMessage(msg, requestHeaders, requestStream, out responseHeaders, out responseStream);
            ProcessResponse(null, responseHeaders, ref responseStream, state);
        }

        public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers,
                                        Stream stream)
        {
            object state = null;
            ProcessRequest(msg, headers, ref stream, ref state);
            sinkStack.Push(this, state);
            NextChannelSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
        }

        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state,
                                         ITransportHeaders headers, Stream stream)
        {
            ProcessResponse(null, headers, ref stream, state);
            sinkStack.AsyncProcessResponse(headers, stream);
        }

        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return NextChannelSink.GetRequestStream(msg, headers);
        }

        public IClientChannelSink NextChannelSink
        {
            get { return base.InternalNextSink as IClientChannelSink; }
        }

        #endregion
    }
}