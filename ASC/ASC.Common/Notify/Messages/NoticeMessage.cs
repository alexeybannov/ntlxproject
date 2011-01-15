#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Messages
{
    [Serializable]
    public class NoticeMessage
        : INoticeMessage
    {
        [NonSerialized] private readonly List<ITagValue> _Arguments = new List<ITagValue>();
        [NonSerialized] private IPattern _Pattern;

        public NoticeMessage()
        {
        }

        public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            Recipient = recipient;
            Action = action;
            ObjectID = objectID;
        }

        public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID, IPattern pattern)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (pattern == null) throw new ArgumentNullException("pattern");
            Recipient = recipient;
            Action = action;
            Pattern = pattern;
            ObjectID = objectID;
            ContentType = pattern.ContentType;
        }

        public NoticeMessage(IDirectRecipient recipient, string subject, string body, string contentType)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (body == null) throw new ArgumentNullException("body");
            Recipient = recipient;
            Subject = subject;
            Body = body;
            ContentType = contentType;
        }

        #region INoticeMessage

        public string ObjectID { get; private set; }
        public IDirectRecipient Recipient { get; private set; }

        public IPattern Pattern
        {
            get { return _Pattern; }
            internal set { _Pattern = value; }
        }

        public INotifyAction Action { get; private set; }

        public ITagValue[] Arguments
        {
            get { return _Arguments.ToArray(); }
        }

        public void AddArgument(params ITagValue[] tagValues)
        {
            if (tagValues == null) throw new ArgumentNullException("tagValues");
            Array.ForEach(
                tagValues,
                tagValue
                =>
                    {
                        if (!_Arguments.Exists(tv => Equals(tv.Tag, tagValue.Tag)))
                            _Arguments.Add(tagValue);
                    }
                );
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string ContentType { get; internal set; }

        public void AddArgument(ITagValue tagValue)
        {
            if (tagValue == null) throw new ArgumentNullException("tagValue");
            if (!_Arguments.Exists(tv => Equals(tv.Tag, tagValue.Tag)))
                _Arguments.Add(tagValue);
        }

        #endregion
    }
}