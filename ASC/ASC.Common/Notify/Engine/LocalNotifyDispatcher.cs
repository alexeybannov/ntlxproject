#region usings

using System;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Engine
{
    public class LocalNotifyDispatcher
        : INotifyDispatcher
    {
        private readonly Context _Context;

        public LocalNotifyDispatcher(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _Context = context;
        }

        #region INotifyDispatcher 

        public void DispatchNotice(INoticeMessage message, string senderName)
        {
            _Context.DispatchEngine.Dispatch(message, senderName, null);
        }

        public SendResponse DispatchNoticeSync(INoticeMessage message, string senderName)
        {
            return _Context.DispatchEngine.Dispatch(message, senderName);
        }

        #endregion
    }
}