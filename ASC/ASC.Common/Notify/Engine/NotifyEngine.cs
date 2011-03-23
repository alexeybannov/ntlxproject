#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ASC.Collections;
using ASC.Common.Utils;
using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using log4net;

#endregion

namespace ASC.Notify.Engine
{
    public delegate void TransferRequest(NotifyEngine sender, NotifyRequest request);

    public class NotifyEngine : INotifyEngine
    {
        private readonly AutoResetEvent PeekQueueEvent = new AutoResetEvent(false);
        private readonly object SyncRoot = new object();
        private readonly Context _Context;
        private readonly Queue<NotifyRequest> _RequestQueue = new Queue<NotifyRequest>(10);
        private readonly EventQueue<SendMethodWrapper> _SendMethodQueue;

        private readonly IPatternFormatter _sysTagFormatter =
            new ReplacePatternFormatter(@"_#(?<tagName>[A-Z0-9_\-.]+)#_", true);

        internal TimeSpan DefaultSleepSpan = TimeSpan.FromSeconds(10);
        private Thread _QueueThread;

        public NotifyEngine(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _SendMethodQueue = new EventQueue<SendMethodWrapper>((smw, date) => smw.NextDateMethod(smw.Method, date));
            _Context = context;
        }

        private void _EnsureNotifyThread()
        {
            if (_QueueThread == null)
            {
                _QueueThread = new Thread(QueueThreadProc);
                _QueueThread.IsBackground = true;
                _QueueThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
                _QueueThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
                _QueueThread.Start(this);
            }
        }

        #region public methods

        public virtual event TransferRequest BeforeTransferRequest;
        public virtual event TransferRequest AfterTransferRequest;

        public virtual NotifyResult Request(NotifyRequest request)
        {
            if (BeforeTransferRequest != null) BeforeTransferRequest(this, request);
            if (AfterTransferRequest != null) AfterTransferRequest(this, request);
            return SendNotify(request);
        }

        public virtual void QueueRequest(NotifyRequest request)
        {
            _EnsureNotifyThread();
            lock (SyncRoot)
            {
                if (BeforeTransferRequest != null)
                    BeforeTransferRequest(this, request);
                _RequestQueue.Enqueue(request);
                PeekQueueEvent.Set();
            }
        }

        public virtual event Action<Context> OnAsyncSendThreadStart;

        public void RegisterSendMethod(INotifyClient client, SendMethod method, SendMethodNextDate nextDateMethod)
        {
            if (client == null) throw new ArgumentNullException("clint");
            if (method == null) throw new ArgumentNullException("method");
            if (nextDateMethod == null) throw new ArgumentNullException("nextDateMethod");
            _EnsureNotifyThread();
            var smw = new SendMethodWrapper(client, method, nextDateMethod);
            lock (SyncRoot)
            {
                _SendMethodQueue.Remove(smw);
                _SendMethodQueue.Enqueue(smw);
            }
            PeekQueueEvent.Set();
        }

        public void UnregisterSendMethod(INotifyClient client, SendMethod method)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (method == null) throw new ArgumentNullException("method");
            var smw = new SendMethodWrapper(client, method, null);
            lock (SyncRoot)
            {
                _SendMethodQueue.Remove(smw);
            }
        }

        #endregion

        #region internal methods

        internal NotifyResult SendNotify(NotifyRequest request)
        {
            SendResponse response = null;
            var sendResponces = new List<SendResponse>();

            response = CheckPreventInterceptors(request, InterceptorPlace.Prepare, null);
            if (response != null)
                sendResponces.Add(response);
            else
            {
                sendResponces.AddRange(SendGroupNotify(request));
            }
            NotifyResult result = DoResultFromResponses(sendResponces);
            try
            {
                LogNoticeResult(request, result);
            }
            catch (Exception exc)
            {
                LogHolder.Log("ASC.Notify").Warn("error on log result", exc);
            }
            return result;
        }

        private void LogNoticeResult(NotifyRequest request, NotifyResult result)
        {
            LogHolder.Log("ASC.Notify").InfoFormat(result.ToString());
        }

        internal SendResponse CheckPreventInterceptors(NotifyRequest request, InterceptorPlace place, string sender)
        {
            if (!request.Intercept(place)) return null;
            return new SendResponse(request.NotifyAction, sender, request.Recipient, SendResult.Prevented);
        }

        internal static NotifyResult DoResultFromResponses(List<SendResponse> sendResponces)
        {
            if (sendResponces == null || sendResponces.Count == 0)
                return new NotifyResult(SendResult.Ok, sendResponces);
            SendResult sres = 0;
            sendResponces.ForEach(sr => sres |= sr.Result);
            return new NotifyResult(sres, sendResponces);
        }

        internal List<SendResponse> SendGroupNotify(NotifyRequest request)
        {
            var responces = new List<SendResponse>();

            SendGroupNotify(request, responces);
            return responces;
        }

        internal void SendGroupNotify(NotifyRequest request, List<SendResponse> responces)
        {
            {
                if (request.Recipient is IDirectRecipient)
                {
                    var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(request.NotifySource);
                    if (
                        !request.IsNeedCheckSubscriptions
                        ||
                        !subscriptionSource.IsUnsubscribe(
                            request.Recipient as IDirectRecipient,
                            request.NotifyAction,
                            request.ObjectID)
                        )
                    {
                        var directresponses = new List<SendResponse>(1);
                        try
                        {
                            directresponses = SendDirectNotify(request);
                        }
                        catch (Exception exc)
                        {
                            var responce = new SendResponse(
                                request.NotifyAction,
                                request.Recipient,
                                exc
                                );
                            directresponses.Add(responce);
                        }
                        responces.AddRange(directresponses);
                    }
                }
                else
                {
                    if (request.Recipient is IRecipientsGroup)
                    {
                        SendResponse checkresp = CheckPreventInterceptors(request, InterceptorPlace.GroupSend, null);
                        if (checkresp != null)
                        {
                            responces.Add(checkresp);
                        }
                        else
                        {
                            var recipientProvider = ProviderResolver.GetEnsure<IRecipientProvider>(request.NotifySource);

                            IRecipient[] recipients = null;
                            try
                            {
                                recipients =
                                    recipientProvider.GetGroupEntries(request.Recipient as IRecipientsGroup,
                                                                      request.ObjectID) ?? new IRecipient[0];
                                foreach (IRecipient recipient in recipients)
                                {
                                    try
                                    {
                                        NotifyRequest newRequest = request.Split(recipient);

                                        SendGroupNotify(newRequest, responces);
                                    }
                                    catch (Exception exc)
                                    {
                                        responces.Add(
                                            new SendResponse(request.NotifyAction, request.Recipient, exc)
                                            );
                                    }
                                }
                            }
                            catch (Exception exc)
                            {
                                responces.Add(
                                    new SendResponse(request.NotifyAction, request.Recipient, exc)
                                        {
                                            Result = SendResult.IncorrectRecipient
                                        }
                                    );
                            }
                        }
                    }
                    else
                    {
                        responces.Add(
                            new SendResponse(request.NotifyAction, request.Recipient, null)
                                {
                                    Result = SendResult.IncorrectRecipient,
                                    Exception =
                                        new NotifyException("recipient may be IRecipientsGroup or IDirectRecipient")
                                }
                            );
                    }
                }
            }
        }

        internal List<SendResponse> SendDirectNotify(NotifyRequest request)
        {
            if (!(request.Recipient is IDirectRecipient))
                throw new ArgumentException("request.Recipient not IDirectRecipient", "request");
            var responses = new List<SendResponse>();

            try
            {
                PrepareRequest(request);
            }
            catch (Exception)
            {
                var resp = new SendResponse(
                    request.NotifyAction,
                    null,
                    request.Recipient,
                    SendResult.Impossible
                    );
            }

            SendResponse response = CheckPreventInterceptors(request, InterceptorPlace.DirectSend, null);
            if (response != null)
            {
                responses.Add(response);
                return responses;
            }
            if (request.SenderNames != null && request.SenderNames.Length > 0)
            {
                foreach (string sendertag in request.SenderNames)
                {
                    ISenderChannel channel = _Context.SenderHolder.GetSender(sendertag);
                    if (channel != null)
                    {
                        try
                        {
                            response = SendDirectNotify(request, channel);
                        }
                        catch (Exception exc)
                        {
                            response = new SendResponse(
                                request.NotifyAction,
                                channel.SenderName,
                                request.Recipient,
                                exc
                                );
                        }
                    }
                    else
                    {
                        response = new SendResponse(request.NotifyAction, sendertag, request.Recipient,
                                                    new NotifyException(
                                                        String.Format(
                                                            "Not registered sender \"{0}\".",
                                                            sendertag)));
                    }
                    responses.Add(response);
                }
            }
            else
            {
                response = new SendResponse(request.NotifyAction, request.Recipient,
                                            new NotifyException("Notice hasn't any senders."));
                responses.Add(response);
            }
            return responses;
        }

        internal SendResponse SendDirectNotify(NotifyRequest request, ISenderChannel channel)
        {
            var recipient = request.Recipient as IDirectRecipient;
            if (recipient == null)
                throw new ArgumentException("request.Recipient not IDirectRecipient", "request");
            request.CurrentSender = channel.SenderName;

            NoticeMessage noticeMessage = null;
            SendResponse oops = CreateNoticeMessageFromNotifyRequest(request, channel.SenderName, out noticeMessage);
            if (oops != null) return oops;
            request.CurrentMessage = noticeMessage;
            SendResponse preventresponse = CheckPreventInterceptors(request, InterceptorPlace.MessageSend,
                                                                    channel.SenderName);
            if (preventresponse != null) return preventresponse;
            if (request.SyncSend)
                return channel.Send(noticeMessage, request.NotifySource);
            else
            {
                channel.SendAsync(noticeMessage);
                return new SendResponse(noticeMessage, channel.SenderName, SendResult.Inprogress);
            }
        }

        internal SendResponse CreateNoticeMessageFromNotifyRequest(NotifyRequest request, string sender,
                                                                   out NoticeMessage noticeMessage)
        {
            var recipientProvider = ProviderResolver.GetEnsure<IRecipientProvider>(request.NotifySource);
            var recipient = request.Recipient as IDirectRecipient;
            string[] addresses = recipient.Addresses;
            if (recipient.Addresses == null || recipient.Addresses.Length == 0)
            {
                addresses = recipientProvider.GetRecipientAddresses(
                    request.Recipient as IDirectRecipient,
                    sender,
                    request.ObjectID
                    );
                recipient = new DirectRecipient(request.Recipient.ID, request.Recipient.Name, addresses);
            }

            noticeMessage = request.CreateMessage(recipient);

            if (request.IsNeedPatternFormatting)
            {
                IPattern pattern = request.GetSenderPattern(sender);
                if (pattern == null)
                    return new SendResponse(request.NotifyAction, sender, recipient,
                                            new NotifyException(
                                                String.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.",
                                                              request.NotifyAction, sender)));

                noticeMessage.Pattern = pattern;
                noticeMessage.ContentType = pattern.ContentType;

                if (request.RequaredTags.Count > 0)
                {
                    var dependencyProvider = ProviderResolver.Get<IDependencyProvider>(request.NotifySource);
                    if (dependencyProvider != null)
                    {
                        ITagValue[] values = null;
                        try
                        {
                            values = dependencyProvider.GetDependencies(
                                noticeMessage,
                                request.ObjectID,
                                request.RequaredTags.ToArray());
                        }
                        catch (Exception exc)
                        {
                            return new SendResponse(noticeMessage, sender, exc);
                        }
                        request.Arguments.AddRange(values ?? new ITagValue[0]);
                    }
                }

                noticeMessage.AddArgument(request.Arguments.ToArray());
                var patternProvider = ProviderResolver.GetEnsure<IPatternProvider>(request.NotifySource);

                IPatternFormatter formatter = patternProvider.GetFormatter(pattern) ?? new NullPatternFormatter();
                try
                {
                    FormatMessage(noticeMessage, formatter);

                    _sysTagFormatter.FormatMessage(
                        noticeMessage, new[]
                                           {
                                               new TagValue(Context._SYS_RECIPIENT_ID, request.Recipient.ID),
                                               new TagValue(Context._SYS_RECIPIENT_NAME, request.Recipient.Name),
                                               new TagValue(Context._SYS_RECIPIENT_ADDRESS,
                                                            (addresses != null && addresses.Length > 0)
                                                                ? addresses[0]
                                                                : null)
                                           }
                        );
                }
                catch (Exception exc)
                {
                    return new SendResponse(request.NotifyAction, sender, recipient, exc);
                }
            }
            return null;
        }

        internal void FormatMessage(NoticeMessage noticeMessage, IPatternFormatter formatter)
        {
            formatter.FormatMessage(noticeMessage);
        }

        #region prep query

        internal void PrepareRequest(NotifyRequest request)
        {
            PrepareRequest_FillSenders(request);

            PrepareRequest_FillPatterns(request);

            PrepareRequest_FillTags(request);
        }

        internal void PrepareRequest_FillSenders(NotifyRequest request)
        {
            if (!request.IsNeedRetriveSenders)
            {
                return;
            }

            var subscriptionSource = ProviderResolver.GetEnsure<ISubscriptionSource>(request.NotifySource);
            request.SenderNames = subscriptionSource.GetSubscriptionMethod(
                request.NotifyAction,
                request.Recipient);
            if (request.SenderNames == null)
                request.SenderNames = new string[0];
        }

        internal void PrepareRequest_FillPatterns(NotifyRequest request)
        {
            if (!request.IsNeedRetrivePatterns)
            {
                return;
            }
            request.Patterns = new IPattern[request.SenderNames.Length];
            if (request.Patterns.Length == 0) return;

            var apProvider = ProviderResolver.GetEnsure<IActionPatternProvider>(request.NotifySource);

            for (int i = 0; i < request.SenderNames.Length; i++)
            {
                string senderName = request.SenderNames[i];

                IPattern pattern =
                    (apProvider.GetPatternMethod != null
                         ? apProvider.GetPatternMethod(request.NotifyAction, senderName, request)
                         : null)
                    ??
                    apProvider.GetPattern(request.NotifyAction, senderName)
                    ??
                    apProvider.GetPattern(request.NotifyAction);

                if (pattern == null)
                    throw new NotifyException(
                        String.Format("For action \"{0}\" by sender \"{1}\" no one patterns getted.",
                                      request.NotifyAction.Name, senderName));
                request.Patterns[i] = pattern;
            }
        }

        internal void PrepareRequest_FillTags(NotifyRequest request)
        {
            if (!request.IsNeedRetriveTags)
            {
                return;
            }

            var patternProvider = ProviderResolver.GetEnsure<IPatternProvider>(request.NotifySource);

            foreach (IPattern pattern in request.Patterns)
            {
                IPatternFormatter formatter = null;

                try
                {
                    formatter = patternProvider.GetFormatter(pattern) ?? new NullPatternFormatter();
                }
                catch (Exception exc)
                {
                    throw new NotifyException(
                        String.Format("For pattern \"{0}\" formatter not instanced.",
                                      pattern), exc);
                }
                var tags = new ITag[0];
                try
                {
                    tags = formatter.GetTags(pattern) ?? new ITag[0];
                }
                catch (Exception exc)
                {
                    throw new NotifyException(
                        String.Format("Get tags from formatter of pattern \"{0}\" failed.",
                                      pattern), exc);
                }

                var notInstancedTags = new List<ITag>();
                foreach (ITag tag in tags)
                {
                    if (
                        !request.Arguments.Exists(tagValue => Equals(tagValue.Tag, tag))
                        &&
                        !request.RequaredTags.Exists(rtag => Equals(rtag, tag))
                        )
                        request.RequaredTags.Add(tag);
                }
            }
        }

        #endregion

        #endregion

        #region streaming methods asynchronously send

        private static void QueueThreadProc(object state)
        {
            var engine = (NotifyEngine) state;
            if (engine.OnAsyncSendThreadStart != null)
                engine.OnAsyncSendThreadStart(engine._Context);
            do
            {
                int count = 0;
                try
                {
                    engine.ChechSendMethodQueue();

                    lock (engine.SyncRoot)
                    {
                        count = engine._RequestQueue.Count;
                    }
                    if (count > 0)
                        engine.SendAllQueueRequests();

                    TimeSpan sleepSpan = engine._SendMethodQueue.FirstEventSpan ?? engine.DefaultSleepSpan;
                    if (sleepSpan > TimeSpan.FromMilliseconds(0))
                    {
                        if (sleepSpan < TimeSpan.FromMilliseconds(1))
                            sleepSpan = TimeSpan.FromMilliseconds(1);
                        engine.PeekQueueEvent.WaitOne(sleepSpan, false);
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine("ERROR in ASC.Notify.Engine.NotifyEngine.QueueThreadProc:" + exc);
                }
            } while (true);
        }

        internal void ChechSendMethodQueue()
        {
            List<SendMethodWrapper> sendMethods = null;
            lock (_SendMethodQueue.SyncRoot)
            {
                int count = _SendMethodQueue.ReadyCount;
                if (count > 0)
                {
                    sendMethods = new List<SendMethodWrapper>(count);
                    while (count-- > 0)
                    {
                        DateTime? date;
                        SendMethodWrapper smw = _SendMethodQueue.Dequeue(out date);
                        if (smw != null)
                        {
                            smw.ScheduleDate = date.Value;
                            sendMethods.Add(smw);
                        }
                    }
                }
            }
            if (sendMethods != null && sendMethods.Count > 0)
            {
                foreach (SendMethodWrapper sendMethodWrapper in sendMethods)
                {
                    try
                    {
                        sendMethodWrapper.Method(sendMethodWrapper.ScheduleDate);
                    }
                    catch
                    {
                    }
                }
            }
        }

        internal void SendAllQueueRequests()
        {
            do
            {
                NotifyRequest request = null;
                lock (SyncRoot)
                {
                    if (_RequestQueue.Count > 0)
                        request = _RequestQueue.Dequeue();
                    else
                        PeekQueueEvent.Reset();
                }
                if (request == null) break;
                if (AfterTransferRequest != null)
                    AfterTransferRequest(this, request);
                try
                {
                    NotifyResult result = SendNotify(request);
                    if (request.NoticeCallback != null)
                        try
                        {
                            request.NoticeCallback(
                                request.NotifyAction,
                                request.ObjectID,
                                request.Recipient,
                                result
                                );
                        }
                        catch
                        {
                        }
                }
                catch
                {
                    lock (SyncRoot) if (_RequestQueue.Count == 0) PeekQueueEvent.Reset();
                    throw;
                }
            } while (true);
        }

        #endregion

        #region send method

        internal class SendMethodWrapper
        {
            public INotifyClient Client;
            public SendMethod Method;
            public SendMethodNextDate NextDateMethod;
            public DateTime ScheduleDate;

            public SendMethodWrapper(INotifyClient client, SendMethod method, SendMethodNextDate nextDateMethod)
            {
                Client = client;
                Method = method;
                NextDateMethod = nextDateMethod;
            }

            public override bool Equals(object obj)
            {
                var smw = (SendMethodWrapper) obj;
                return Client.Equals(smw.Client) && Method.Equals(smw.Method);
            }

            public override int GetHashCode()
            {
                return Client.GetHashCode() | Method.GetHashCode();
            }
        }

        #endregion
    }
}