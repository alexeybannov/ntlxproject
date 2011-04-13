using System.Collections.Generic;

namespace ASC.Core
{
    public interface ISubscriptionService
    {
        IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId);

        IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId);

        SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId);

        void SaveSubscription(SubscriptionRecord s);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId);


        IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId);

        void SetSubscriptionMethod(SubscriptionMethod m);
    }
}
