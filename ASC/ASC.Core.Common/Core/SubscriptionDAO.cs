using System;
using System.Collections.Generic;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using System.Configuration;

namespace ASC.Core
{
    class DbSubscriptionService : DbBaseService, ISubscriptionService
    {
        public DbSubscriptionService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

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

        public string[] GetSubscriptionMethod(int tenant, string sourceID, string actionID, string recipientID)
        {
            var method = DbManager.ExecuteScalar<string>(
                Query("core_subscriptionmethod")
                .Select("Sender")
                .Where("Source", sourceID)
                .Where("Action", actionID)
                .Where(Exp.Eq("Recipient", recipientID) | Exp.Eq("Recipient", Guid.Empty.ToString()))//recipient == Guid.Empty - default
                .OrderBy("Recipient", false)
                .SetMaxResults(1)
            );
            return string.IsNullOrEmpty(method) ? new string[0] : method.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void UpdateSubscriptionMethod(int tenant, string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            if (senderNames == null || senderNames.Length == 0)
            {
                DbManager.ExecuteNonQuery(
                    Delete("core_subscriptionmethod").Where("Source", sourceID).Where("Action", actionID).Where("Recipient", recipientID)
                );
            }
            else
            {
                DbManager.ExecuteNonQuery(
                    Insert("core_subscriptionmethod")
                    .InColumns("Source", "Action", "Recipient", "Sender")
                    .Values(sourceID, actionID, recipientID, string.Join("|", senderNames))
                );
            }
        }

        public List<Subscription> LoadSubscriptions(int tenant, string sourceID, string actionID, bool actionFilter, string objectID, bool objectFilter, string recipientID, bool recipientFilter)
        {
            var query = Query("core_subscription").Select("Id", "Source", "Action", "Recipient", "Object", "Unsubscribed").Where("Source", sourceID);
            if (actionFilter) query.Where("Action", actionID);
            if (recipientFilter) query.Where("Recipient", recipientID);
            if (objectFilter) query.Where("Object", objectID);

            return DbManager
                .ExecuteList(query)
                .ConvertAll<Subscription>(r =>
                {
                    return new Subscription()
                    {
                        ID = (int)Convert.ToInt64(r[0]),
                        SourceID = (string)r[1],
                        ActionID = (string)r[2],
                        RecipientID = (string)r[3],
                        ObjectID = (string)r[4],
                        IsUnsubscribed = Convert.ToBoolean(r[5]),
                    };
                });
        }

        public void Subscribe(int tenant, string sourceID, string actionID, string recipientID, string objectID)
        {
            var exists = LoadSubscriptions(sourceID, actionID, true, objectID, true, recipientID, true);
            if (exists.Count == 0)
            {
                DbManager.ExecuteNonQuery(
                    Insert("core_subscription").InColumns("Source", "Action", "Recipient", "Object", "Unsubscribed")
                    .Values(sourceID, actionID, recipientID, !string.IsNullOrEmpty(objectID) ? objectID : null, 0)
                );
            }
            else
            {
                if (exists[0].IsUnsubscribed)
                {
                    DbManager.ExecuteNonQuery(Update("core_subscription").Set("Unsubscribed", 0).Where("Id", exists[0].ID));
                }
            }
        }

        public void UnSubscribe(int tenant, string sourceID, string actionID, string objectID, bool objectFilter)
        {
            var delete = Delete("core_subscription").Where("Source", sourceID).Where("Action", actionID);
            if (objectFilter) delete.Where("Object", objectID);
            DbManager.ExecuteNonQuery(delete);
        }
        public void UnSubscribe(int tenant, string sourceID, string actionID, string recipientID, string objectID)
        {
            var exists = LoadSubscriptions(sourceID, actionID, true, objectID, true, recipientID, true);
            if (exists.Count == 0)
            {
                DbManager.ExecuteNonQuery(
                    Insert("core_subscription").InColumns("Source", "Action", "Recipient", "Object", "Unsubscribed")
                    .Values(sourceID, actionID, recipientID, objectID, 1)
                );
            }
            else
            {
                if (!exists[0].IsUnsubscribed)
                {
                    DbManager.ExecuteNonQuery(Update("core_subscription").Set("Unsubscribed", 1).Where("Id", exists[0].ID));
                }
            }
        }

        [Serializable]
        public class Subscription
        {
            internal int ID
            {
                get;
                set;
            }

            public Subscription()
            {
                IsUnsubscribed = false;
            }

            public string SourceID;

            public string ActionID;

            public string RecipientID;

            public string ObjectID;

            public bool IsUnsubscribed;
        }
    }
}