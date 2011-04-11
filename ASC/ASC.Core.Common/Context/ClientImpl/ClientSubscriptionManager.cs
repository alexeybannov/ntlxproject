using System;
using System.Linq;

namespace ASC.Core
{
    class ClientSubscriptionManager : ISubscriptionManagerClient
    {
        private readonly ISubscriptionService service;


        public ClientSubscriptionManager(ISubscriptionService service)
        {
            if (service == null) throw new ArgumentNullException("subscriptionManager");
            this.service = service;
        }

        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            service.SetSubscription(GetTenant(), sourceID, actionID, objectID, recipientID, true);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            service.SetSubscription(GetTenant(), sourceID, actionID, objectID, recipientID, false);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID, objectID);
        }

        public void UnsubscribeAll(string sourceID, string actionID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID);
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            var s = service.GetSubscriptionsByRecipient(GetTenant(), sourceID, actionID, recipientID).FirstOrDefault();
            if (s == null) s = service.GetSubscriptionsByRecipient(GetTenant(), sourceID, actionID, Guid.Empty.ToString()).FirstOrDefault();
            return s == null ? new string[0] : s.Methods;
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            return service.GetSubscriptionsByObject(GetTenant(), sourceID, actionID, objectID)
                .Select(s => s.RecipientId)
                .ToArray();
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
        {
            return service.GetSubscriptionsByRecipient(GetTenant(), sourceID, actionID, recipientID)
                .Select(s => s.ObjectId)
                .ToArray();
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            var s = service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, objectID);
            return s != null && s.Subscribed;
        }

        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            service.SetSubscriptionMethod(GetTenant(), sourceID, actionID, recipientID, senderNames);
        }


        private int GetTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}
