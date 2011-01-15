#region usings

using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Recipients;
using ASC.Notify.Sinks;

#endregion

namespace ASC.Notify
{
    public interface INotifyService
    {
        SendResponse DispatchDirectNotice(IDirectRecipient recipient, string subject, string body, string senderName);

        void RegisterSender(ISenderChannel channel);

        void RegisterSender(string senderName, ISenderSink senderSink);

        void RegisterClientSender(string senderName);

        void UnregisterSender(string senderName);

        INotifyClient RegisterClient(INotifySource source);
    }
}