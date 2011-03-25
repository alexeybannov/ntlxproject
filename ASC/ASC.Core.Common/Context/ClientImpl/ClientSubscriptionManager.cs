using System;

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
            service.Subscribe(GetTenant(), sourceID, actionID, objectID, recipientID, true);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            service.Subscribe(GetTenant(), sourceID, actionID, objectID, recipientID, false);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            service.UnsubscribeAll(GetTenant(), sourceID, actionID, objectID);
        }

        public void UnsubscribeAll(string sourceID, string actionID)
        {
            service.UnsubscribeAll(GetTenant(), sourceID, actionID);
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            return service.GetSubscriptionMethod(GetTenant(), sourceID, actionID, recipientID);
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            return service.GetRecipients(GetTenant(), sourceID, actionID, objectID);
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
        {
            return service.GetSubscriptions(GetTenant(), sourceID, actionID, recipientID);
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            return service.IsUnsubscribe(GetTenant(), sourceID, recipientID, actionID, objectID);
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
