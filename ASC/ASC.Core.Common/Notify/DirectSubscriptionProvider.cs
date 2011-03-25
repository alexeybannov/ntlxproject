using System;
using System.Collections.Generic;
using ASC.Notify.Model;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    class DirectSubscriptionProvider : ISubscriptionProvider
    {
        private readonly IRecipientProvider _recipientProvider;
        private readonly IActionProvider _actionProvider;
        private readonly ISubscriptionManagerClient _subscriptionManager;
        private readonly string sourceID;


        internal DirectSubscriptionProvider(string sourceID, ISubscriptionManagerClient subscriptionManager, IRecipientProvider recipientProvider, IActionProvider actionProvider)
        {
            if (String.IsNullOrEmpty(sourceID)) throw new ArgumentNullException("sourceID");
            if (subscriptionManager == null) throw new ArgumentNullException("subscriptionManager");
            if (recipientProvider == null) throw new ArgumentNullException("recipientProvider");
            if (actionProvider == null) throw new ArgumentNullException("actionProvider");
            
            this.sourceID = sourceID;
            _subscriptionManager = subscriptionManager;
            _recipientProvider = recipientProvider;
            _actionProvider = actionProvider;
        }


        public IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            
            var recipientIDs = _subscriptionManager.GetRecipients(sourceID, action.ID, objectID) ?? new string[0];
            var recipients = new List<IRecipient>(recipientIDs.Length);
            foreach (var recipientID in recipientIDs)
            {
                var recipient = _recipientProvider.GetRecipient(recipientID);
                if (recipient != null) recipients.Add(recipient);
            }
            return recipients.ToArray();
        }

        public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            return _subscriptionManager.GetSubscriptionMethod(sourceID, action.ID, recipient.ID);
        }

        public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            return _subscriptionManager.IsUnsubscribe(sourceID, recipient.ID, action.ID, objectID);
        }        

        public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            _subscriptionManager.Subscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            _subscriptionManager.Unsubscribe(sourceID, action.ID, objectID, recipient.ID);
        }

        public void UnSubscribe(INotifyAction action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _subscriptionManager.UnsubscribeAll(sourceID, action.ID);
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            _subscriptionManager.UnsubscribeAll(sourceID, action.ID, objectID);
        }

        public void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            throw new NotSupportedException("use UnSubscribe(INotifyAction, string, IRecipient )");
        }

        public string[] GetSubscriptions(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            return _subscriptionManager.GetSubscriptions(sourceID, action.ID, recipient.ID);
        }

        public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            _subscriptionManager.UpdateSubscriptionMethod(sourceID, action.ID, recipient.ID, senderNames);
        }
    }
}