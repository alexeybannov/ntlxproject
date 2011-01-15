#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify
{
    public class PureNoticeMessage
    {
        internal PureNoticeMessage()
        {
        }

        public PureNoticeMessage(string subject, string body, IEnumerable<IRecipient> recipients)
        {
            if (recipients == null) throw new ArgumentNullException("recipients");
            Subject = subject;
            Body = body;
            Recipients = new List<IRecipient>(recipients);
            ContentType = Pattern.TextContentType;
        }

        public List<IRecipient> Recipients { get; internal set; }

        public string Subject { get; internal set; }

        public string Body { get; internal set; }

        public string ContentType { get; set; }
    }
}