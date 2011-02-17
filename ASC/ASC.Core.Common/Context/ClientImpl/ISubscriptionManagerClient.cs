
namespace ASC.Core
{
    public interface ISubscriptionManagerClient
    {
        #region identify ways to subscribe

        void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames);

        string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID);

        #endregion

        #region Subscription and evasion of notices

        void Subscribe(string sourceID, string actionID, string objectID, string recipientID);

        void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID);

        void Unsubscribe(string sourceID, string actionID, string recipientID);

        void UnsubscribeAll(string sourceID, string actionID, string objectID);

        void UnsubscribeAll(string sourceID, string actionID);

        #endregion

        #region obtaining subscriptions

        string[] GetRecipients(string sourceID, string actionID, string objectID);

        bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID);

        string[] GetSubscriptions(string sourceID, string actionID, string recipientID);

        #endregion

        int Version { get; }
    }
}