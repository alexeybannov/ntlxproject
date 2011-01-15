#region usings

using ASC.Notify.Messages;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Model
{
    public interface IDependencyProvider
    {
        ITagValue[] GetDependencies(INoticeMessage message, string objectID, ITag[] tags);
    }
}