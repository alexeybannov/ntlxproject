#region usings

using System.Collections;
using ASC.Notify.Messages;
using ASC.Notify.Model;

#endregion

namespace ASC.Notify.Channels
{
    public interface ISenderChannel
    {
        string SenderName { get; }
        IDictionary Properties { get; }
        SendResponse Send(INoticeMessage message, INotifySource source);
        void SendAsync(INoticeMessage message);
        SendResponse DirectSend(INoticeMessage message);
    }
}