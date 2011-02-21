using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Factories;
using ASC.Core.Users.DAO;
using ASC.Core.Users.Service.SrvImpl;
using log4net;

[assembly: AssemblyServices(typeof(SubscriptionManager))]

namespace ASC.Core.Users.Service.SrvImpl
{
    class SubscriptionManager : RemotingServiceController, ISubscriptionManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SubscriptionManager));
        private readonly IDAOFactory factory;
        private int version = 0;

        internal SubscriptionManager(IDAOFactory daoFactory)
            : base(Constants.SubscriptionManagerServiceInfo)
        {
            if (daoFactory == null) throw new ArgumentNullException("daoFactory");
            factory = daoFactory;
        }

        #region ISubscriptionManager
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.Subscribe(sourceID, actionID, recipientID, objectID);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.UnSubscribe(sourceID, actionID, recipientID, objectID);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void UnsubscribeAll(string sourceID, string actionID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.UnSubscribe(sourceID, actionID, null, false);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.UnSubscribe(sourceID, actionID, objectID, true);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void Unsubscribe(string sourceID, string actionID, string recipientID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.UnSubscribe(sourceID, actionID, recipientID);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            Interlocked.Increment(ref version);

            using (var dao = GetDAO())
            {
                dao.UpdateSubscriptionMethod(sourceID, actionID, recipientID, senderNames);
            }
        }


        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");

            List<Subscription> subscriptions = null;
            using (var dao = GetDAO())
            {
                subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, objectID, true, null, false);
            }

            List<string> recipients = new List<string>(subscriptions.Count);
            subscriptions.ForEach(sub => { if (!sub.IsUnsubscribed && !recipients.Contains(sub.RecipientID)) recipients.Add(sub.RecipientID); });

            return recipients.ToArray();
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            using (var dao = GetDAO())
            {
                return dao.GetSubscriptionMethod(sourceID, actionID, recipientID);
            }
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            List<Subscription> subscriptions = null;
            using (var dao = GetDAO())
            {
                subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, null, false, recipientID, true);
            }

            List<string> objects = new List<string>(subscriptions.Count);
            foreach (var sub in subscriptions)
                if (!sub.IsUnsubscribed && !objects.Contains(sub.ObjectID)) objects.Add(sub.ObjectID);

            return objects.ToArray();
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            if (sourceID == null) throw new ArgumentNullException("sourceID");
            if (actionID == null) throw new ArgumentNullException("actionID");
            if (recipientID == null) throw new ArgumentNullException("recipientID");

            List<Subscription> subscriptions = null;
            using (var dao = GetDAO())
            {
                subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, objectID, true, recipientID, true);
            }
            return (subscriptions.Count == 1) && subscriptions[0].IsUnsubscribed;
        }

        public int Version
        {
            get { return version; }
        }

        #endregion

        private ISubscriptionDAO GetDAO()
        {
            return factory.GetSubscriptionDao();
        }
    }
}