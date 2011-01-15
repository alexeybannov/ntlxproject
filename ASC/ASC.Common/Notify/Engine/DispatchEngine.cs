#region usings

using System;
using System.Collections.Generic;
using System.Threading;
using ASC.Common.Utils;
using ASC.Notify.Channels;
using ASC.Notify.Messages;
using ASC.Threading;

#endregion

namespace ASC.Notify.Engine
{
    public delegate void DispatchCallback(INoticeMessage message, SendResponse responce);

    public class DispatchEngine
    {
        private static readonly ManualResetEvent _fakeWaitHandle = new ManualResetEvent(false);
        private static TimeSpan _sendAttemptInterval = TimeSpan.FromMilliseconds(1000*10);
        private readonly Context _Context;
        private readonly Queue<DispatchRequest> _DispatchQueue = new Queue<DispatchRequest>(10);
        private readonly MultiAttemptTaskQueue _Queue;

        internal DispatchEngine(Context context)
        {
            if (context == null) throw new ArgumentNullException("context");
            _Context = context;
            _Queue = new MultiAttemptTaskQueue(
                1,
                5,
                _sendAttemptInterval);
        }

        public SendResponse Dispatch(INoticeMessage message, string senderName)
        {
            if (_Context.SenderHolder.GetSender(senderName) == null)
                return new SendResponse(message, senderName, SendResult.Impossible);

            SendResponse response = null;
            var request =
                new DispatchRequest(
                    message,
                    _Context.SenderHolder.GetSender(senderName),
                    (nm, res) => response = res
                    ) {SendAttemptInterval = (int) _sendAttemptInterval.TotalMilliseconds};
            DispatchExecutor(request);
            return response;
        }

        public void Dispatch(INoticeMessage message, string senderName, DispatchCallback callback)
        {
            var request =
                new DispatchRequest(
                    message,
                    _Context.SenderHolder.GetSender(senderName),
                    callback) {SendAttemptInterval = (int) _sendAttemptInterval.TotalMilliseconds};
            _Queue.EnqueueTask(DispatchExecutor, request);
        }

        #region helpers

        private class DispatchRequest
        {
            internal static int _dispatchNum;

            public readonly int DispatchNum;
            public readonly INoticeMessage NoticeMessage;
            public readonly int SendAttemptCount = 5;
            public readonly ISenderChannel SenderChannel;
            public int AttemptCount;
            public DispatchCallback DispatchCallback;
            public DateTime LastAttempt = DateTime.MinValue;
            public int SendAttemptInterval = 500;
            public int SleepBeforeSend = 200;

            public DispatchRequest(INoticeMessage message, ISenderChannel senderChannel, DispatchCallback callback)
            {
                DispatchNum = Interlocked.Increment(ref _dispatchNum);
                NoticeMessage = message;
                SenderChannel = senderChannel;
                DispatchCallback = callback;
            }
        }

        #endregion

        #region

        private static bool DispatchExecutor(object request)
        {
            var dispatchRequest = (DispatchRequest) request;
            if (dispatchRequest.SleepBeforeSend > 0)
                Thread.Sleep(dispatchRequest.SleepBeforeSend);
            dispatchRequest.LastAttempt = DateTime.UtcNow;
            dispatchRequest.AttemptCount++;
            SendResponse response = null;
            if (dispatchRequest.SenderChannel == null)
                response = new SendResponse(dispatchRequest.NoticeMessage, null, SendResult.Impossible);
            else
                response = dispatchRequest.SenderChannel.DirectSend(dispatchRequest.NoticeMessage);
            string logmsg = String.Format(
                "#{4}: [{0}] sended to [{1}] over {2}, status:{3} "
                , dispatchRequest.NoticeMessage.Subject
                , dispatchRequest.NoticeMessage.Recipient.Name
                , (dispatchRequest.SenderChannel != null ? dispatchRequest.SenderChannel.SenderName : "")
                , response.Result,
                dispatchRequest.DispatchNum);

            if (response.Result == SendResult.Inprogress)
            {
                dispatchRequest.DispatchCallback = null;
                LogHolder.Log("ASC.Notify.Dispatch")
                    .Debug(logmsg, response.Exception);
                LogHolder.Log("ASC.Notify.Dispatch")
                    .Debug(String.Format("attemt #{1}, try send after {0}",
                                         TimeSpan.FromMilliseconds(dispatchRequest.SendAttemptInterval),
                                         dispatchRequest.AttemptCount));
            }
            else if (response.Result == SendResult.Impossible)
            {
                LogHolder.Log("ASC.Notify.Dispatch").Error(logmsg, response.Exception);
            }
            else
            {
                LogHolder.Log("ASC.Notify.Dispatch").Debug(logmsg);
            }
            if (dispatchRequest.DispatchCallback != null)
                dispatchRequest.DispatchCallback(dispatchRequest.NoticeMessage, response);
            return !(response.Result == SendResult.Inprogress);
        }

        private static void DispatchMethod(object request)
        {
            var dispatchRequest = (DispatchRequest) request;
        }

        private static void DispatchMethod(object request, bool timedout)
        {
            DispatchMethod(request);
        }

        #endregion
    }
}