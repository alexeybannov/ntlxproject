#region usings

using System;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Patterns
{
    public sealed class DummyPatternFormatter
        : IPatternFormatter
    {
        public static readonly Tag SubjectTag = new Tag("DummySubject");

        public static readonly Tag BodyTag = new Tag("DummyBody");

        #region IPatternFormatter

        public ITag[] GetTags(IPattern pattern)
        {
            return new ITag[] {SubjectTag, BodyTag};
        }

        public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Pattern == null) throw new ArgumentException("message");
            if (tagsValues == null) throw new ArgumentNullException("tagsValues");
            if (tagsValues.Length != 2) throw new ArgumentException("tagsValues");
            if (tagsValues[0].Tag != SubjectTag || tagsValues[1].Tag != BodyTag)
                throw new ArgumentException("tagsValues");
            message.Subject = Convert.ToString(tagsValues[0].Value);
            message.Body = Convert.ToString(tagsValues[1].Value);
        }

        public void FormatMessage(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            FormatMessage(message, message.Arguments);
        }

        #endregion
    }
}