#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Services;
using ASC.Core.Users;

#endregion

namespace ASC.Core
{
    internal class ClientSubscriptionManager : ISubscriptionManager
    {
        private readonly ISubscriptionManager subscriptionManager;
        private readonly object syncRoot = new object();

        private readonly Dictionary<SubItem, VersionedElement<string[]>> _sendMethod =
            new Dictionary<SubItem, VersionedElement<string[]>>(50);

        private readonly Dictionary<SubObject, VersionedElement<string[]>> _recipients =
            new Dictionary<SubObject, VersionedElement<string[]>>(50);

        private readonly Dictionary<SubItem, VersionedElement<string[]>> _subscriptions =
            new Dictionary<SubItem, VersionedElement<string[]>>(50);

        private readonly Dictionary<SubUnsub, VersionedElement<bool>> _unsubscriptions =
            new Dictionary<SubUnsub, VersionedElement<bool>>(50);

        internal TimeSpan _actualPeriod = TimeSpan.FromSeconds(5);
        internal DateTime _lastVersionCheck = DateTime.MinValue;
        internal int _actualVersion;

        public IServiceInfo Info
        {
            get { throw new NotSupportedException(); }
        }

        public ClientSubscriptionManager(ISubscriptionManager subscriptionManager)
        {
            if (subscriptionManager == null) throw new ArgumentNullException("subscriptionManager");
            this.subscriptionManager = subscriptionManager;
        }

        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            ClearCache();
            subscriptionManager.Subscribe(sourceID, actionID, objectID, recipientID);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            ClearCache();
            subscriptionManager.Unsubscribe(sourceID, actionID, objectID, recipientID);
        }

        public void Unsubscribe(string sourceID, string actionID, string recipientID)
        {
            ClearCache();
            subscriptionManager.Unsubscribe(sourceID, actionID, recipientID);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            ClearCache();
            subscriptionManager.UnsubscribeAll(sourceID, actionID, objectID);
        }

        public void UnsubscribeAll(string sourceID, string actionID)
        {
            ClearCache();
            subscriptionManager.UnsubscribeAll(sourceID, actionID);
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            var item = new SubItem(sourceID, actionID, recipientID);
            VersionedElement<string[]> value;
            bool cached = false;
            lock (syncRoot)
            {
                cached = _sendMethod.TryGetValue(item, out value);
                if (!cached || !value.IsActual(Version))
                {
                    string[] methods = subscriptionManager.GetSubscriptionMethod(item.sourceID, item.actionID,
                                                                                 item.recipientID);
                    if (methods == null) methods = new string[0];
                    value = new VersionedElement<string[]>(Version, methods);
                    if (cached) _sendMethod[item] = value;
                    else _sendMethod.Add(item, value);
                }
            }
            return value.Value != null ? (string[]) value.Value.Clone() : null;
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            var item = new SubObject(sourceID, actionID, objectID);
            VersionedElement<string[]> value;
            bool cached = false;
            lock (syncRoot)
            {
                cached = _recipients.TryGetValue(item, out value);
                if (!cached || !value.IsActual(Version))
                {
                    value = new VersionedElement<string[]>(
                        Version,
                        subscriptionManager.GetRecipients(item.sourceID, item.actionID, item.objectID));
                    if (cached) _recipients[item] = value;
                    else _recipients.Add(item, value);
                }
            }
            return value.Value != null ? (string[]) value.Value.Clone() : new string[0];
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
        {
            var item = new SubItem(sourceID, actionID, recipientID);
            VersionedElement<string[]> value;
            bool cached = false;
            lock (syncRoot)
            {
                cached = _subscriptions.TryGetValue(item, out value);
                if (!cached || !value.IsActual(Version))
                {
                    value = new VersionedElement<string[]>(
                        Version,
                        subscriptionManager.GetSubscriptions(item.sourceID, item.actionID, item.recipientID));
                    if (cached) _subscriptions[item] = value;
                    else _subscriptions.Add(item, value);
                }
            }
            return value.Value != null ? (string[]) value.Value.Clone() : null;
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            var item = new SubUnsub(sourceID, actionID, recipientID, objectID);
            VersionedElement<bool> value;
            bool cached = false;
            lock (syncRoot)
            {
                cached = _unsubscriptions.TryGetValue(item, out value);
                if (!cached || !value.IsActual(Version))
                {
                    value = new VersionedElement<bool>(
                        Version,
                        subscriptionManager.IsUnsubscribe(item.sourceID, item.recipientID, item.actionID, item.objectID));
                    if (cached) _unsubscriptions[item] = value;
                    else _unsubscriptions.Add(item, value);
                }
            }
            return value.Value;
        }

        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            subscriptionManager.UpdateSubscriptionMethod(sourceID, actionID, recipientID, senderNames);
            var item = new SubItem(sourceID, actionID, recipientID);
            var value = new VersionedElement<string[]>(Version, senderNames);
            lock (syncRoot)
            {
                if (_sendMethod.ContainsKey(item))
                    _sendMethod[item] = value;
                else
                    _sendMethod.Add(item, value);
            }
        }

        public int Version
        {
            get
            {
                if (DateTime.Now - _actualPeriod > _lastVersionCheck)
                {
                    _lastVersionCheck = DateTime.Now;
                    _actualVersion = subscriptionManager.Version;
                }
                return _actualVersion;
            }
        }

        private void ClearCache()
        {
            lock (syncRoot)
            {
                _recipients.Clear();
                _subscriptions.Clear();
                _unsubscriptions.Clear();
            }
        }

        #region internal methods

        private class SubAction
        {
            public SubAction(string source, string action)
            {
                if (String.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
                if (String.IsNullOrEmpty(action)) throw new ArgumentNullException("action");
                sourceID = source;
                actionID = action;
            }

            public readonly string sourceID;
            public readonly string actionID;

            public override bool Equals(object obj)
            {
                var item = obj as SubAction;
                return item != null && (sourceID == item.sourceID && actionID == item.actionID);
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}:{1}", sourceID, actionID);
            }
        }

        private class SubItem : SubAction
        {
            public SubItem(string source, string action, string recipient)
                : base(source, action)
            {
                if (String.IsNullOrEmpty(recipient)) throw new ArgumentNullException("recipient");
                recipientID = recipient;
            }

            public readonly string recipientID;

            public override bool Equals(object obj)
            {
                var item = obj as SubItem;
                return item != null &&
                       (sourceID == item.sourceID && actionID == item.actionID && recipientID == item.recipientID);
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}:{1}:{2}", sourceID, actionID, recipientID);
            }
        }

        private class SubObject : SubAction
        {
            public SubObject(string source, string action, string obj)
                : base(source, action)
            {
                objectID = obj;
            }

            public readonly string objectID;

            public override bool Equals(object obj)
            {
                var item = obj as SubObject;
                return item != null &&
                       (sourceID == item.sourceID && actionID == item.actionID && objectID == item.objectID);
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}:{1}:{2}", sourceID, actionID, objectID);
            }
        }

        private class SubUnsub : SubItem
        {
            public SubUnsub(string source, string action, string recipient, string obj)
                : base(source, action, recipient)
            {
                objectID = obj;
            }

            public readonly string objectID;

            public override bool Equals(object obj)
            {
                var item = obj as SubUnsub;
                return item != null &&
                       (sourceID == item.sourceID && actionID == item.actionID && recipientID == item.recipientID &&
                        objectID == item.objectID);
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public override string ToString()
            {
                return String.Format("{0}:{1}:{2}:{3}", sourceID, actionID, recipientID, objectID);
            }
        }

        private class VersionedElement<TElement>
        {
            public VersionedElement(int version, TElement value)
            {
                Value = value;
                Version = version;
            }

            public readonly TElement Value;
            public readonly int Version;

            public bool IsActual(int masterVersion)
            {
                return Version == masterVersion;
            }
        }

        #endregion
    }
}