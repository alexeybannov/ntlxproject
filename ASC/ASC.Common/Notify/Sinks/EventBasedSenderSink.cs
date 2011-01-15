#region usings

using System;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Sinks
{
    public delegate SendResponse NotifySendCallback(INoticeMessage message);

    public sealed class EventBasedSenderSink
        : SinkSkeleton,
          ISenderSink
    {
        private readonly NotifySendCallback _sendFunction;

        public EventBasedSenderSink(NotifySendCallback sendFunction)
        {
            if (sendFunction == null) throw new ArgumentNullException("sendFunction");
            _sendFunction = sendFunction;
        }

        #region ISenderSink Members

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            return _sendFunction(message);
        }

        #endregion
    }
}