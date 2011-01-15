#region usings

using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Patterns
{
    public interface IPatternFormatter
    {
        ITag[] GetTags(IPattern pattern);

        void FormatMessage(INoticeMessage message, ITagValue[] tagsValues);

        void FormatMessage(INoticeMessage message);
    }
}