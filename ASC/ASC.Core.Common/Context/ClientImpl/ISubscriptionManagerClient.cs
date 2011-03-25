using System;

namespace ASC.Core
{
    interface ISubscriptionManagerClient
    {
        string[] GetRecipients(string sourceID, string actionID, string objectID);

        string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID);

        string[] GetSubscriptions(string sourceID, string actionID, string recipientID);

        bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID);

        void Subscribe(string sourceID, string actionID, string objectID, string recipientID);

        void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID);

        void UnsubscribeAll(string sourceID, string actionID, string objectID);

        void UnsubscribeAll(string sourceID, string actionID);

        void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames);
    }
}
