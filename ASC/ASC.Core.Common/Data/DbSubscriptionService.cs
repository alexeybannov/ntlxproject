using System;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;

namespace ASC.Core.Data
{
    class DbSubscriptionService : DbBaseService, ISubscriptionService
    {
        public DbSubscriptionService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }


        public string[] GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");
            if (recipientId == null) throw new ArgumentNullException("recipientId");
            
            var q = Query("core_subscription", tenant)
                .Select("object")
                .Where("source", sourceId)
                .Where("action", actionId)
                .Where("recipient", recipientId)
                .Where("unsubscribed", false)
                .GroupBy(1);
            return ExecList(q).Select(r => ToString(r)).ToArray();
        }

        public string[] GetRecipients(int tenant, string sourceId, string actionId, string objectId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");
            
            var q = Query("core_subscription", tenant)
                .Select("recipient")
                .Where("source", sourceId)
                .Where("action", actionId)
                .Where("object", objectId ?? string.Empty)
                .Where("unsubscribed", false)
                .GroupBy(1);
            return ExecList(q).Select(r => ToString(r)).ToArray();
        }


        public string[] GetSubscriptionMethod(int tenant, string sourceId, string actionId, string recipientId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");
            if (recipientId == null) throw new ArgumentNullException("recipientId");
            
            var q = Query("core_subscriptionmethod", tenant)
                .Select("sender")
                .Where("source", sourceId)
                .Where("action", actionId)
                .Where(Exp.Eq("recipient", recipientId) | Exp.Eq("recipient", Guid.Empty.ToString()))//recipient == Guid.Empty - default
                .OrderBy("recipient", false)
                .SetMaxResults(1);

            return (ExecScalar<string>(q) ?? string.Empty).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void SetSubscriptionMethod(int tenant, string sourceId, string actionId, string recipientId, string[] senders)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");
            if (recipientId == null) throw new ArgumentNullException("recipientId");
            
            var i = senders == null || senders.Length == 0 ?
                (ISqlInstruction)Delete("core_subscriptionmethod", tenant).Where("source", sourceId).Where("action", actionId).Where("recipient", recipientId) :
                (ISqlInstruction)Insert("core_subscriptionmethod", tenant).InColumns("source", "action", "recipient", "sender").Values(sourceId, actionId, recipientId, string.Join("|", senders));
            ExecNonQuery(i);
        }


        public bool IsUnsubscribe(int tenant, string sourceId, string recipientId, string actionId, string objectId)
        {
            var q = Query("core_subscription", tenant)
                .Select("unsubscribed")
                .Where("source", sourceId)
                .Where("action", actionId)
                .Where("recipient", recipientId)
                .Where("object", objectId ?? string.Empty)
                .SetMaxResults(1);
            return ExecScalar<int>(q) == 1;
        }

        public void Subscribe(int tenant, string sourceId, string actionId, string objectId, string recipientId, bool subscribe)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");
            if (recipientId == null) throw new ArgumentNullException("recipientId");

            var i = Insert("core_subscription", tenant)
                .InColumns("source", "action", "recipient", "object", "unsubscribed")
                .Values(sourceId, actionId, recipientId, objectId ?? string.Empty, !subscribe);
            ExecNonQuery(i);
        }

        public void UnsubscribeAll(int tenant, string sourceId, string actionId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            var d = Delete("core_subscription", tenant).Where("source", sourceId).Where("action", actionId);
            ExecNonQuery(d);
        }

        public void UnsubscribeAll(int tenant, string sourceId, string actionId, string objectId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            var d = Delete("core_subscription", tenant).Where("source", sourceId).Where("action", actionId).Where("object", objectId ?? string.Empty);
            ExecNonQuery(d);
        }


        private string ToString(object[] r)
        {
            return string.Empty.Equals((string)r[0]) ? null : (string)r[0];
        }
    }
}
