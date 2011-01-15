#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify
{
    public delegate void SendNoticeCallback(
        INotifyAction action, string objectID, IRecipient recipient, NotifyResult result);

    public delegate void SendMethod(DateTime scheduleDate);

    public delegate DateTime? SendMethodNextDate(SendMethod method, DateTime fromDate);

    public interface INotifyClient
    {
        #region

        void SetStaticTags(IEnumerable<ITagValue> tagValues);

        #endregion

        #region

        NotifyResult SendDirectNotice(PureNoticeMessage message, string[] senderNames);

        void SendDirectNoticeAsync(PureNoticeMessage message, SendNoticeCallback sendCallback, string[] senderNames);

        NotifyResult SendNoticeTo(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames,
                                  params ITagValue[] args);

        void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients, string[] senderNames,
                               SendNoticeCallback sendCallback, params ITagValue[] args);

        NotifyResult SendNoticeTo(INotifyAction action, string objectID, IRecipient[] recipients,
                                  params ITagValue[] args);

        void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients,
                               SendNoticeCallback sendCallback, params ITagValue[] args);

        NotifyResult SendNotice(INotifyAction action, string objectID, params ITagValue[] args);

        void SendNoticeAsync(INotifyAction action, string objectID, SendNoticeCallback sendCallback,
                             params ITagValue[] args);

        NotifyResult SendNotice(INotifyAction action, string objectID, IRecipient recipient, params ITagValue[] args);

        void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient,
                             SendNoticeCallback sendCallback, params ITagValue[] args);

        #endregion

        #region

        [Obsolete("use RegisterSendMethod")]
        string RegisterPullNotice(string cronString, INotifyAction action, string objectID,
                                  SendNoticeCallback sendCallback, params ITagValue[] args);

        [Obsolete("use RegisterSendMethod")]
        void UnregisterPullNotice(string cookie);

        void RegisterSendMethod(SendMethod method, TimeSpan period);

        void RegisterSendMethod(SendMethod method, TimeSpan period, DateTime startDate);

        void RegisterSendMethod(SendMethod method, string cronString);

        void RegisterSendMethod(SendMethod method, SendMethodNextDate nextDateMethod);

        void UnregisterSendMethod(SendMethod method);

        #endregion

        #region     callback     

        void BeginSingleRecipientEvent(string name);

        void EndSingleRecipientEvent(string name);

        void AddInterceptor(ISendInterceptor interceptor);

        void RemoveInterceptor(string name);

        #endregion
    }
}