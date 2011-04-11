using System;
using System.Linq;
using ASC.Notify.Model;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    class DirectSubscriptionProvider : ISubscriptionProvider
    {
        private readonly IRecipientProvider recipientProvider;
        private readonly IActionProvider actionProvider;
        private readonly ISubscriptionManagerClient subscriptionManager;
        private readonly string sourceID;


        public DirectSubscriptionProvider(string sourceID, ISubscriptionManagerClient subscriptionManager, IRecipientProvider recipientProvider, IActionProvider actionProvider)
        {
            if (string.IsNullOrEmpty(sourceID)) throw new ArgumentNullException("sourceID");
            if (subscriptionManager == null) throw new ArgumentNullException("subscriptionManager");
            if (recipientProvider == null) throw new ArgumentNullException("recipientProvider");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            
            this.sourceID = sourceID;
            this.subscriptionManager = subscriptionManager;
            this.recipientProvider = recipientProvider;
            this.actionProvider = actionProvider;
        }


        public string[] GetSubscriptions(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            return subscriptionManager.GetSubscriptions(sourceID, action.ID, recipient.ID);
        }

        public IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");

            return subscriptionManager.GetRecipients(sourceID, action.ID, objectID)
                .Select(r => recipientProvider.GetRecipient(r))
                .Where(r => r != null)
                .ToArray();
        }

        public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            return subscriptionManager.GetSubscriptionMethod(sourceID, action.ID, recipient.ID);
        }
        
        public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            subscriptionManager.UpdateSubscriptionMethod(sourceID, action.ID, recipient.ID, senderNames);
        }
        
        public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (action == null) throw new ArgumentNullException("action");
            
            return subscriptionManager.IsUnsubscribe(sourceID, recipient.ID, action.ID, objectID);
        }        

        public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            subscriptionManager.Subscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            
            subscriptionManager.Unsubscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action)
        {
            if (action == null) throw new ArgumentNullException("action");
            
            subscriptionManager.UnsubscribeAll(sourceID, action.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            
            subscriptionManager.UnsubscribeAll(sourceID, action.ID, objectID);
        }

        [Obsolete("Use UnSubscribe(INotifyAction, string, IRecipient)", true)]
        public void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            throw new NotSupportedException("use UnSubscribe(INotifyAction, string, IRecipient )");
        }
    }
}