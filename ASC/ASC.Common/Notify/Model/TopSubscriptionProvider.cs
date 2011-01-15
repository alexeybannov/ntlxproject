#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    public class TopSubscriptionProvider : ISubscriptionProvider
    {
        private readonly string[] _defaultSenderMethods = new string[0];
        private readonly ISubscriptionProvider _directSubscriptionProvider;
        private readonly IRecipientProvider _recipientProvider;

        public TopSubscriptionProvider(IRecipientProvider recipientProvider,
                                       ISubscriptionProvider directSubscriptionProvider)
        {
            if (recipientProvider == null)
                throw new ArgumentNullException("recipientProvider");
            if (directSubscriptionProvider == null)
                throw new ArgumentNullException("directSubscriptionProvider");
            _recipientProvider = recipientProvider;
            _directSubscriptionProvider = directSubscriptionProvider;
        }

        public TopSubscriptionProvider(IRecipientProvider recipientProvider,
                                       ISubscriptionProvider directSubscriptionProvider, string[] defaultSenderMethods)
            : this(recipientProvider, directSubscriptionProvider)
        {
            _defaultSenderMethods = defaultSenderMethods;
        }

        #region ISubscriptionSource

        public virtual string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            string[] senders = _directSubscriptionProvider.GetSubscriptionMethod(action, recipient);
            if (senders == null || senders.Length == 0)
            {
                List<IRecipient> parents = WalkUp(recipient);
                foreach (IRecipient parent in parents)
                {
                    senders = _directSubscriptionProvider.GetSubscriptionMethod(action, parent);
                    if (senders != null && senders.Length != 0) break;
                }
            }
            if (senders == null || senders.Length == 0)
                senders = _defaultSenderMethods;
            return senders;
        }

        public virtual IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            var recipents = new List<IRecipient>(5);
            IRecipient[] directRecipients = _directSubscriptionProvider.GetRecipients(action, objectID) ??
                                            new IRecipient[0];
            recipents.AddRange(directRecipients);
            return recipents.ToArray();
        }

        public virtual bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            return _directSubscriptionProvider.IsUnsubscribe(recipient, action, objectID);
        }

        #endregion

        #region ISubscriptionProvider

        public virtual void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            _directSubscriptionProvider.Subscribe(action, objectID, recipient);
        }

        public virtual void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            _directSubscriptionProvider.UnSubscribe(action, objectID, recipient);
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _directSubscriptionProvider.UnSubscribe(action, objectID);
        }

        public void UnSubscribe(INotifyAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            _directSubscriptionProvider.UnSubscribe(action);
        }

        public virtual void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            string[] objects = GetSubscriptions(action, recipient);
            foreach (string objectID in objects)
                _directSubscriptionProvider.UnSubscribe(action, objectID, recipient);
        }

        public virtual void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient,
                                                     params string[] senderNames)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            if (senderNames == null)
                throw new ArgumentNullException("senderNames");
            _directSubscriptionProvider.UpdateSubscriptionMethod(action, recipient, senderNames);
        }

        public virtual string[] GetSubscriptions(INotifyAction action, IRecipient recipient)
        {
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            if (action == null)
                throw new ArgumentNullException("action");
            var objects = new List<string>();
            string[] direct = _directSubscriptionProvider.GetSubscriptions(action, recipient) ?? new string[0];
            MergeObjects(objects, direct);
            List<IRecipient> parents = WalkUp(recipient);
            foreach (IRecipient parent in parents)
            {
                direct = _directSubscriptionProvider.GetSubscriptions(action, parent) ?? new string[0];
                if (recipient is IDirectRecipient)
                {
                    foreach (string groupsubscr in direct)
                    {
                        if (
                            !objects.Contains(groupsubscr)
                            &&
                            !_directSubscriptionProvider.IsUnsubscribe(recipient as IDirectRecipient, action,
                                                                       groupsubscr))
                            objects.Add(groupsubscr);
                    }
                }
                else
                    MergeObjects(objects, direct);
            }
            return objects.ToArray();
        }

        #endregion

        internal List<IRecipient> WalkUp(IRecipient recipient)
        {
            var parents = new List<IRecipient>();
            IRecipientsGroup[] groups = _recipientProvider.GetGroups(recipient) ?? new IRecipientsGroup[0];
            foreach (IRecipientsGroup group in groups)
            {
                parents.Add(group);
                List<IRecipient> topgroups = WalkUp(group);
                parents.AddRange(topgroups);
            }
            return parents;
        }

        internal void MergeActions(List<INotifyAction> result, IEnumerable<INotifyAction> additions)
        {
            foreach (INotifyAction addition in additions)
                if (!result.Contains(addition)) result.Add(addition);
        }

        internal void MergeObjects(List<string> result, IEnumerable<string> additions)
        {
            foreach (string addition in additions)
                if (!result.Contains(addition)) result.Add(addition);
        }
    }
}