using System.Collections.Generic;

namespace ASC.Core
{
    public interface ISubscriptionService
    {
        IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId);

        IEnumerable<SubscriptionRecord> GetSubscriptionsByRecipient(int tenant, string sourceId, string actionId, string recipientId);

        IEnumerable<SubscriptionRecord> GetSubscriptionsByObject(int tenant, string sourceId, string actionId, string objectId);

        SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId);

        void SetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId, bool subcribe);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId);
    }
}
