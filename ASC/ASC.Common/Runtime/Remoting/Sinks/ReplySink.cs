#region usings

using System;
using System.IO;
using System.Runtime.Remoting.Messaging;

#endregion

namespace ASC.Runtime.Remoting.Sinks
{
    internal class ReplySink : IMessageSink
    {
        private readonly IMessageSink nextSink;
        private readonly SinkBase parentSink;
        private readonly object state;

        public ReplySink(IMessageSink nextSink, SinkBase parentSink, object state)
        {
            this.nextSink = nextSink;
            this.parentSink = parentSink;
            this.state = state;
        }

        #region IMessageSink Members

        public IMessage SyncProcessMessage(IMessage msg)
        {
            Stream stream = null;
            parentSink.ProcessResponse(msg, null, ref stream, state);
            return nextSink.SyncProcessMessage(msg);
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            throw new NotSupportedException("AsyncProcessMessage method not supported.");
        }

        public IMessageSink NextSink
        {
            get { return nextSink; }
        }

        #endregion
    }
}