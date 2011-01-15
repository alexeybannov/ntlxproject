#region usings

using System;
using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Notify.Sinks;

#endregion

namespace ASC.Notify.Model
{
    internal class NotifyServiceImpl
        : INotifyService
    {
        internal Context _Context;

        public NotifyServiceImpl(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _Context = context;
        }

        #region INotifyService

        public SendResponse DispatchDirectNotice(IDirectRecipient recipient, string subject, string body,
                                                 string senderName)
        {
            return
                _Context.NotifyDispatcher.DispatchNoticeSync(
                    new NoticeMessage(recipient, subject, body, Pattern.TextContentType), senderName);
        }

        public void RegisterSender(ISenderChannel channel)
        {
            _Context.SenderHolder.RegisterSender(channel);
        }

        public void RegisterSender(string senderName, ISenderSink senderSink)
        {
            _Context.SenderHolder.RegisterSender(
                new CustomeSenderChannel(_Context, senderName, senderSink)
                );
        }

        public void RegisterClientSender(string senderName)
        {
            _Context.SenderHolder.RegisterSender(
                new CustomeSenderChannel(_Context, senderName, new MockSenderSink())
                );
        }

        public void UnregisterSender(string senderName)
        {
            ISenderChannel senderChannel = _Context.SenderHolder.GetSender(senderName);
            if (senderChannel != null)
                _Context.SenderHolder.UngeristerSender(senderChannel);
        }

        public INotifyClient RegisterClient(INotifySource source)
        {
            var client = new NotifyClientImpl(_Context, source);

            _Context.Invoke_NotifyClientRegistration(client);
            return client;
        }

        #endregion
    }
}