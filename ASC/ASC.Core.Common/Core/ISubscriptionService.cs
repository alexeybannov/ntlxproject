﻿
namespace ASC.Core
{
    public interface ISubscriptionService
    {
        string[] GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId);

        string[] GetRecipients(int tenant, string sourceId, string actionId, string objectId);

        
        string[] GetSubscriptionMethod(int tenant, string sourceId, string actionId, string recipientId);

        void UpdateSubscriptionMethod(int tenant, string sourceId, string actionId, string recipientId, string[] senderNames);

        
        bool IsUnsubscribe(int tenant, string sourceId, string recipientId, string actionId, string objectId);
                
        void Subscribe(int tenant, string sourceId, string actionId, string objectId, string recipientId);

        void Unsubscribe(int tenant, string sourceId, string actionId, string objectId, string recipientId);

        void UnsubscribeAll(int tenant, string sourceId, string actionId);

        void UnsubscribeAll(int tenant, string sourceId, string actionId, string objectId);
    }
}
