#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Cron;
using ASC.Notify.Engine;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    internal class NotifyClientImpl : INotifyClient
    {
        private readonly Context ctx;
        private readonly InterceptorStorage interceptors = new InterceptorStorage();
        private readonly INotifySource notifySource;
        private readonly List<ITagValue> staticTags = new List<ITagValue>();
        private readonly object syncRoot = new object();

        internal NotifyClientImpl(Context context, INotifySource notifySource)
        {
            if (notifySource == null) throw new ArgumentNullException("notifySource");
            if (context == null) throw new ArgumentNullException("context");
            this.notifySource = notifySource;
            ctx = context;
        }

        #region INotifyClient

        public void SetStaticTags(IEnumerable<ITagValue> tagValues)
        {
            lock (syncRoot)
            {
                if (tagValues != null)
                {
                    staticTags.AddRange(tagValues);
                }
                else
                {
                    staticTags.Clear();
                }
            }
        }

        public NotifyResult SendDirectNotice(PureNoticeMessage message, string[] senderNames)
        {
            if (senderNames == null) throw new ArgumentNullException("senderNames");
            if (message == null) throw new ArgumentNullException("message");
            return IterateSendForRecipients(
                rec => { return Send(new NotifyRequest(notifySource, message, rec, senderNames)); },
                message.Recipients
                );
        }

        public void SendDirectNoticeAsync(PureNoticeMessage message, SendNoticeCallback sendCallback,
                                          string[] senderNames)
        {
            if (senderNames == null) throw new ArgumentNullException("senderNames");
            if (message == null) throw new ArgumentNullException("message");
            IterateSendForRecipients(
                r =>
                    {
                        var request = new NotifyRequest(notifySource, message, r, senderNames);
                        request.NoticeCallback = sendCallback;
                        SendAsync(request);
                        return null;
                    },
                message.Recipients
                );
        }

        public NotifyResult SendNoticeTo(INotifyAction action, string objectID, IRecipient[] recipients,
                                         string[] senderNames, params ITagValue[] args)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            if (senderNames == null) throw new ArgumentNullException("senderNames");
            return IterateSendForRecipients(
                r =>
                    {
                        NotifyRequest request = CreateRequest(action, objectID, r, null, args, senderNames);
                        request.IsNeedCheckSubscriptions = false;
                        return Send(request);
                    },
                recipients
                );
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients,
                                      string[] senderNames, SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            if (senderNames == null) throw new ArgumentNullException("senderNames");
            IterateSendForRecipients(
                (rec) =>
                    {
                        NotifyRequest request =
                            CreateRequest(
                                action,
                                objectID,
                                rec,
                                sendCallback,
                                args,
                                senderNames);
                        request.IsNeedCheckSubscriptions = false;
                        SendAsync(request);
                        return null;
                    },
                recipients
                );
        }

        public NotifyResult SendNoticeTo(INotifyAction action, string objectID, IRecipient[] recipients,
                                         params ITagValue[] args)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            return
                IterateSendForRecipients(
                    (rec) =>
                        {
                            NotifyRequest request =
                                CreateRequest(
                                    action,
                                    objectID,
                                    rec,
                                    null,
                                    args,
                                    null);
                            request.IsNeedCheckSubscriptions = false;
                            return Send(request);
                        },
                    recipients
                    );
        }

        public void SendNoticeToAsync(INotifyAction action, string objectID, IRecipient[] recipients,
                                      SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            IterateSendForRecipients(
                (rec) =>
                    {
                        NotifyRequest request =
                            CreateRequest(
                                action,
                                objectID,
                                rec,
                                sendCallback,
                                args,
                                null);
                        request.IsNeedCheckSubscriptions = false;
                        SendAsync(request);
                        return null;
                    },
                recipients
                );
        }

        public NotifyResult SendNotice(INotifyAction action, string objectID, params ITagValue[] args)
        {
            var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(notifySource);
            IRecipient[] recipients = subscriptionSource.GetRecipients(action, objectID);
            return SendNoticeTo(action, objectID, recipients, args);
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, SendNoticeCallback sendCallback,
                                    params ITagValue[] args)
        {
            var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(notifySource);
            IRecipient[] recipients = subscriptionSource.GetRecipients(action, objectID);
            SendNoticeToAsync(action, objectID, recipients, sendCallback, args);
        }

        public NotifyResult SendNotice(INotifyAction action, string objectID, IRecipient recipient,
                                       params ITagValue[] args)
        {
            return
                IterateSendForRecipients(
                    (rec) =>
                        {
                            NotifyRequest request =
                                CreateRequest(
                                    action,
                                    objectID,
                                    rec,
                                    null,
                                    args,
                                    null);
                            request.IsNeedCheckSubscriptions = false;
                            return Send(request);
                        },
                    new[] {recipient}
                    );
        }

        public void SendNoticeAsync(INotifyAction action, string objectID, IRecipient recipient,
                                    SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            IterateSendForRecipients(
                (rec) =>
                    {
                        NotifyRequest request =
                            CreateRequest(
                                action,
                                objectID,
                                rec,
                                sendCallback,
                                args,
                                null);
                        request.IsNeedCheckSubscriptions = false;
                        SendAsync(request);
                        return null;
                    },
                new[] {recipient}
                );
        }

        #region pull 

        public string RegisterPullNotice(string cronString, INotifyAction action, string objectID,
                                         SendNoticeCallback sendCallback, params ITagValue[] args)
        {
            throw new NotSupportedException("use RegisterSendMethod");
        }

        public void UnregisterPullNotice(string cookie)
        {
            throw new NotSupportedException("use RegisterSendMethod");
        }

        public void RegisterSendMethod(SendMethod method, TimeSpan period)
        {
            RegisterSendMethod(method, period, DateTime.UtcNow);
        }

        public void RegisterSendMethod(SendMethod method, TimeSpan period, DateTime startDate)
        {
            RegisterSendMethod(method, (mtd, dt) => (dt < startDate) ? startDate : dt + period);
        }

        public void RegisterSendMethod(SendMethod method, string cronString)
        {
            var expr = new CronExpression(cronString);
            RegisterSendMethod(method, (mtd, dt) => expr.GetTimeAfter(dt));
        }

        public void RegisterSendMethod(SendMethod method, SendMethodNextDate nextDateMethod)
        {
            ctx.NotifyEngine.RegisterSendMethod(this, method, nextDateMethod);
        }

        public void UnregisterSendMethod(SendMethod method)
        {
            ctx.NotifyEngine.UnregisterSendMethod(this, method);
        }

        #endregion

        #region     callback     

        public void BeginSingleRecipientEvent(string name)
        {
            interceptors.Add(new SingleRecipientInterceptor(name));
        }

        public void EndSingleRecipientEvent(string name)
        {
            interceptors.Remove(name);
        }

        public void AddInterceptor(ISendInterceptor interceptor)
        {
            interceptors.Add(interceptor);
        }

        public void RemoveInterceptor(string name)
        {
            interceptors.Remove(name);
        }

        #endregion

        #endregion

        #region

        internal NotifyResult IterateSendForRecipients(IterateSend iterateSend, IEnumerable<IRecipient> recipients)
        {
            var result = new NotifyResult();
            try
            {
                BeginSingleRecipientEvent("__syspreventduplicateinterceptor");
                foreach (IRecipient recipient in recipients)
                {
                    NotifyResult sendResult = iterateSend(recipient);
                    if (sendResult != null) result.Merge(sendResult);
                }
            }
            finally
            {
            }
            return result;
        }

        internal NotifyResult Send(NotifyRequest request)
        {
            request.SyncSend = true;
            return _Send(request, false);
        }

        internal void SendAsync(NotifyRequest request)
        {
            _Send(request, true);
        }

        internal NotifyResult _Send(NotifyRequest request, bool async)
        {
            request.Interceptors = interceptors.GetAll();

            AddStaticTegs(request);
            NotifyResult result = null;
            if (!async)
            {
                result = ctx.NotifyEngine.Request(request);
            }
            else
            {
                ctx.NotifyEngine.QueueRequest(request);
            }
            return result;
        }

        internal void AddStaticTegs(NotifyRequest request)
        {
            lock (syncRoot)
            {
                foreach (ITagValue tagvalue in staticTags)
                {
                    if (tagvalue != null && !request.Arguments.Exists(tv => tv.Tag.Name == tagvalue.Tag.Name))
                        request.Arguments.Add(tagvalue);
                }
            }
        }

        internal NotifyRequest CreateRequest(INotifyAction action, string objectID, IRecipient recipient,
                                             SendNoticeCallback sendCallback, ITagValue[] args, string[] senders)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            var request = new NotifyRequest(notifySource, action, objectID, recipient);
            request.SenderNames = senders;
            request.NoticeCallback = sendCallback;
            if (args != null) request.Arguments.AddRange(args);
            return request;
        }

        internal delegate NotifyResult IterateSend(IRecipient recipient);

        #endregion
    }
}