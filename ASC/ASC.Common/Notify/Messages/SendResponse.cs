#region usings

using System;
using ASC.Notify.Model;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Messages
{
    [Serializable]
    public class SendResponse
    {
        public SendResponse()
        {
            Result = SendResult.Ok;
        }

        public SendResponse(INotifyAction action, IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            Exception = exc;
            Recipient = recipient;
            NotifyAction = action;
        }

        public SendResponse(IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            Exception = exc;
            Recipient = recipient;
        }

        public SendResponse(INotifyAction action, string senderName, IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            SenderName = senderName;
            Exception = exc;
            Recipient = recipient;
            NotifyAction = action;
        }

        public SendResponse(string senderName, IRecipient recipient, Exception exc)
        {
            Result = SendResult.Impossible;
            SenderName = senderName;
            Exception = exc;
            Recipient = recipient;
        }

        public SendResponse(INotifyAction action, string senderName, IRecipient recipient, SendResult sendResult)
        {
            SenderName = senderName;
            Recipient = recipient;
            Result = sendResult;
            NotifyAction = action;
        }

        public SendResponse(string senderName, IRecipient recipient, SendResult sendResult)
        {
            SenderName = senderName;
            Recipient = recipient;
            Result = sendResult;
        }

        public SendResponse(INoticeMessage message)
        {
            NoticeMessage = message;
            if (message != null)
            {
                Recipient = message.Recipient;
                NotifyAction = message.Action;
            }
        }

        public SendResponse(INoticeMessage message, string sender, SendResult result)
        {
            NoticeMessage = message;
            SenderName = sender;
            Result = result;
            if (message != null)
            {
                Recipient = message.Recipient;
                NotifyAction = message.Action;
            }
        }

        public SendResponse(INoticeMessage message, string sender, Exception exc)
        {
            NoticeMessage = message;
            SenderName = sender;
            Result = SendResult.Impossible;
            Exception = exc;
            if (message != null)
            {
                Recipient = message.Recipient;
                NotifyAction = message.Action;
            }
        }

        public INoticeMessage NoticeMessage { get; internal set; }

        public INotifyAction NotifyAction { get; internal set; }

        public SendResult Result { get; internal set; }

        public Exception Exception { get; internal set; }

        public string SenderName { get; internal set; }

        public IRecipient Recipient { get; internal set; }
    }
}