#region usings

using System;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Sinks
{
    internal class MockSenderSink
        : SinkSkeleton,
          ISenderSink
    {
        #region ISenderSink Members

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            throw new ApplicationException("incorrect registered dispatcher");
        }

        #endregion
    }
}