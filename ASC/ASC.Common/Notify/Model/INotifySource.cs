#region usings

using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    public interface INotifySource
    {
        string ID { get; }

        string Name { get; }

        IActionPatternProvider GetActionPatternProvider();

        IActionProvider GetActionProvider();

        IPatternProvider GetPatternProvider();

        IDependencyProvider GetDependencyProvider();

        IRecipientProvider GetRecipientsProvider();

        ISubscriptionProvider GetSubscriptionProvider();

        ISubscriptionSource GetSubscriptionSource();
    }
}