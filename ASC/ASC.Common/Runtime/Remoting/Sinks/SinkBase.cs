#region usings

using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    public abstract class SinkBase : BaseChannelObjectWithProperties, IMessageSink
    {
        private readonly object perProviderState;
        private object nextSink;

        public SinkBase()
        {
            perProviderState = CallContext.GetData("perProviderState");
            CallContext.SetData("perProviderState", null);
        }

        protected object PerProviderState
        {
            get { return perProviderState; }
        }

        protected object InternalNextSink
        {
            get { return nextSink; }
        }

        #region IMessageSink Members

        public IMessage SyncProcessMessage(IMessage msg)
        {
            object state = null;
            Stream stream = null;
            ProcessRequest(msg, null, ref stream, ref state);
            IMessage responseMessage = NextSink.SyncProcessMessage(msg);
            stream = null;
            ProcessResponse(responseMessage, null, ref stream, state);
            return responseMessage;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            object state = null;
            Stream stream = null;
            ProcessRequest(msg, null, ref stream, ref state);
            var localReplySink = new ReplySink(replySink, this, state);
            return NextSink.AsyncProcessMessage(msg, localReplySink);
        }

        public IMessageSink NextSink
        {
            get { return nextSink as IMessageSink; }
        }

        #endregion

        public void SetNextSink(object nextSink)
        {
            this.nextSink = nextSink;
        }

        public virtual void ProcessRequest(IMessage message, ITransportHeaders headers,
                                           ref Stream stream, ref object state)
        {
        }

        public virtual void ProcessResponse(IMessage message, ITransportHeaders headers,
                                            ref Stream stream, object state)
        {
        }
    }
}