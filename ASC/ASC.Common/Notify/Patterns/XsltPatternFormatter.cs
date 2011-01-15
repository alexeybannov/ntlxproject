#region usings

using System;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Patterns
{
    public sealed class XsltPatternFormatter
        : IPatternFormatter
    {
        #region IPatternFormatter

        public ITag[] GetTags(IPattern pattern)
        {
            throw new NotImplementedException();
        }

        public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
        {
            throw new NotImplementedException();
        }

        public void FormatMessage(INoticeMessage message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}