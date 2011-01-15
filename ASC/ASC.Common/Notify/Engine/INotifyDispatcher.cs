#region usings

using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Engine
{
    public interface INotifyDispatcher
    {
        void DispatchNotice(INoticeMessage message, string senderName);

        SendResponse DispatchNoticeSync(INoticeMessage message, string senderName);
    }
}