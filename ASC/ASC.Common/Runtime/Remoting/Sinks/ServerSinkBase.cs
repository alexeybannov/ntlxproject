#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    public class ServerSinkBase : SinkBase, IServerChannelSink
    {
        #region IServerChannelSink Members

        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack,
                                               IMessage requestMsg, ITransportHeaders requestHeaders,
                                               Stream requestStream,
                                               out IMessage responseMsg, out ITransportHeaders responseHeaders,
                                               out Stream responseStream)
        {
            object state = null;
            ProcessRequest(requestMsg, requestHeaders, ref requestStream, ref state);
            sinkStack.Push(this, state);
            ServerProcessing serverProcessing = NextChannelSink.ProcessMessage(sinkStack,
                                                                               requestMsg, requestHeaders, requestStream,
                                                                               out responseMsg, out responseHeaders,
                                                                               out responseStream);
            if (serverProcessing == ServerProcessing.Complete)
                ProcessResponse(responseMsg, responseHeaders, ref responseStream, state);
            return serverProcessing;
        }

        public void AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack,
                                         object state, IMessage msg, ITransportHeaders headers, Stream stream)
        {
            ProcessResponse(msg, headers, ref stream, state);
            sinkStack.AsyncProcessResponse(msg, headers, stream);
        }

        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack,
                                        object state, IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        public IServerChannelSink NextChannelSink
        {
            get { return base.InternalNextSink as IServerChannelSink; }
        }

        #endregion
    }
}