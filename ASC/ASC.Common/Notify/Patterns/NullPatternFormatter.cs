#region usings

using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Patterns
{
    internal class NullPatternFormatter
        : IPatternFormatter
    {
        #region IPatternFormatter Members

        public ITag[] GetTags(IPattern pattern)
        {
            return new ITag[0];
        }

        public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
        {
        }

        public void FormatMessage(INoticeMessage message)
        {
        }

        #endregion
    }
}