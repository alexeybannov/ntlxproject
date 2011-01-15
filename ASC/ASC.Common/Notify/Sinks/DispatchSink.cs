#region usings

using System;
using ASC.Notify.Engine;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Sinks
{
    internal class DispatchSink
        : SinkSkeleton
    {
        private readonly string _ParentSenderName;
        private INotifyDispatcher _Dispatcher;

        public DispatchSink(string parentSenderName, INotifyDispatcher dispatcher)
        {
            SwitchDispatcher(dispatcher);
            _ParentSenderName = parentSenderName;
        }

        public void SwitchDispatcher(INotifyDispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            _Dispatcher = dispatcher;
        }

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            return _Dispatcher.DispatchNoticeSync(message, _ParentSenderName);
        }

        public override void ProcessMessageAsync(INoticeMessage message)
        {
            _Dispatcher.DispatchNotice(message, _ParentSenderName);
        }
    }
}