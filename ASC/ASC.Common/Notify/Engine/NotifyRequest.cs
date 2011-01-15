#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Engine
{
    public class NotifyRequest
    {
        public NotifyRequest(INotifySource notifySource, PureNoticeMessage pureMessage, IRecipient recipient,
                             string[] senders)
            : this()
        {
            if (notifySource == null) throw new ArgumentNullException("notifySource");
            if (pureMessage == null) throw new ArgumentNullException("pureMessage");
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (senders == null) throw new ArgumentNullException("senders");
            NotifySource = notifySource;
            Recipient = recipient;
            PureMessage = pureMessage;
            SenderNames = senders;
        }

        public NotifyRequest(INotifySource notifySource, INotifyAction action, string objectID, IRecipient recipient)
            : this()
        {
            if (notifySource == null) throw new ArgumentNullException("notifySource");
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            NotifySource = notifySource;
            Recipient = recipient;
            NotifyAction = action;
            ObjectID = objectID;
            NoticeCallback = null;
        }

        private NotifyRequest()
        {
            Properties = new Hashtable();
            Arguments = new List<ITagValue>();
            RequaredTags = new List<ITag>();
            Interceptors = new List<ISendInterceptor>();
        }

        #region public properties

        public INotifySource NotifySource { get; internal set; }

        public INotifyAction NotifyAction { get; internal set; }
        public string ObjectID { get; internal set; }

        public IRecipient Recipient { get; internal set; }

        public List<ITagValue> Arguments { get; internal set; }

        public SendNoticeCallback NoticeCallback { get; internal set; }

        public string CurrentSender { get; internal set; }
        public INoticeMessage CurrentMessage { get; internal set; }

        public Hashtable Properties { get; private set; }

        #endregion

        #region internal fields

        internal PureNoticeMessage PureMessage { get; set; }

        internal string[] SenderNames { get; set; }
        internal IPattern[] Patterns { get; set; }

        internal List<ITag> RequaredTags { get; set; }

        public object Credentials { get; set; }

        internal List<ISendInterceptor> Interceptors { get; set; }

        #endregion

        #region helpers

        private bool? _IsNeedCheckSubscriptions;
        private bool? _IsNeedPatternFormatting;
        private bool? _IsNeedRetriveTags;
        private bool _syncSend;

        internal bool IsNeedRetriveSenders
        {
            get { return SenderNames == null; }
        }

        internal bool IsNeedRetrivePatterns
        {
            get { return PureMessage == null && Patterns == null; }
        }

        internal bool IsNeedRetriveTags
        {
            get
            {
                if (_IsNeedRetriveTags.HasValue) return _IsNeedRetriveTags.Value;
                return PureMessage == null;
            }
            set { _IsNeedRetriveTags = value; }
        }

        internal bool IsNeedPatternFormatting
        {
            get
            {
                if (_IsNeedPatternFormatting.HasValue) return _IsNeedPatternFormatting.Value;
                return PureMessage == null;
            }
            set { _IsNeedPatternFormatting = value; }
        }

        internal bool IsNeedCheckSubscriptions
        {
            get
            {
                if (_IsNeedCheckSubscriptions.HasValue) return _IsNeedCheckSubscriptions.Value;
                return PureMessage == null;
            }
            set { _IsNeedCheckSubscriptions = value; }
        }

        internal bool SyncSend
        {
            get { return _syncSend || NoticeCallback != null; }
            set { _syncSend = value; }
        }

        internal bool Intercept(InterceptorPlace place)
        {
            bool result = false;
            Interceptors.ForEach(
                inter =>
                    {
                        if ((inter.PreventPlace & place) == place)
                        {
                            try
                            {
                                result = (inter.PreventSend(this, place) || result);
                            }
                            catch
                            {
                            }
                        }
                    }
                );
            return result;
        }

        internal IPattern GetSenderPattern(string senderName)
        {
            if (
                SenderNames == null || Patterns == null
                ||
                SenderNames.Length == 0 || Patterns.Length == 0
                ||
                SenderNames.Length != Patterns.Length
                )
                return null;
            int index = Array.IndexOf(SenderNames, senderName);
            if (index < 0)
                throw new ApplicationException(String.Format("Sender with tag {0} dnot found", senderName));
            return Patterns[index];
        }

        internal NotifyRequest Split(IRecipient recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            var newRequest = new NotifyRequest(NotifySource, NotifyAction, ObjectID, recipient);
            newRequest.NoticeCallback = NoticeCallback;
            newRequest.SenderNames = SenderNames;
            newRequest.Patterns = Patterns;
            newRequest.Arguments = new List<ITagValue>(Arguments);
            newRequest.RequaredTags = RequaredTags;
            newRequest.PureMessage = PureMessage;
            newRequest.CurrentSender = CurrentSender;
            newRequest.CurrentMessage = CurrentMessage;
            newRequest.Interceptors.AddRange(Interceptors);
            newRequest._syncSend = _syncSend;
            return newRequest;
        }

        internal NoticeMessage CreateMessage(IDirectRecipient recipient)
        {
            if (PureMessage != null)
                return new NoticeMessage(recipient, PureMessage.Subject, PureMessage.Body, PureMessage.ContentType);
            else
                return new NoticeMessage(recipient, NotifyAction, ObjectID);
        }

        #endregion
    }
}