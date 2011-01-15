#region usings

using System;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    public interface ISubscriptionProvider
        : ISubscriptionSource
    {
        void Subscribe(INotifyAction action, string objectID, IRecipient recipient);

        void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient);

        void UnSubscribe(INotifyAction action, string objectID);

        void UnSubscribe(INotifyAction action);

        void UnSubscribe(INotifyAction action, IRecipient recipient);

        void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames);

        string[] GetSubscriptions(INotifyAction action, IRecipient recipient);
    }

    public static class SubscriptionProviderHelper
    {
        public static bool IsSubscribed(this ISubscriptionProvider provider, INotifyAction action, IRecipient recipient,
                                        string objectID)
        {
            return Array.Exists(
                provider.GetSubscriptions(action, recipient),
                id => id == objectID || (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(objectID))
                );
        }
    }
}