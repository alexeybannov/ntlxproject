using System;
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Data;
using System.Collections.Generic;

namespace ASC.Core.Data
{
    class DbSubscriptionService : DbBaseService, ISubscriptionService
    {
        public DbSubscriptionService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {

        }


        public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
        {
            var q = GetQuery(tenant, sourceId, actionId);
            return ExecList(q).ConvertAll(r => ToSubscriptionRecord(r));
        }

        public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId)
        {
            var q = GetQuery(tenant, sourceId, actionId);
            if (!string.IsNullOrEmpty(recipientId)) q.Where("recipient", recipientId);
            else q.Where("object", objectId ?? string.Empty);

            return ExecList(q).ConvertAll(r => ToSubscriptionRecord(r));
        }

        public SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId)
        {
            if (recipientId == null) throw new ArgumentNullException("recipientId");

            var q = GetQuery(tenant, sourceId, actionId)
                .Where("recipient", recipientId)
                .Where("object", objectId ?? string.Empty);

            return ExecList(q).ConvertAll(r => ToSubscriptionRecord(r)).FirstOrDefault();
        }

        public void SaveSubscription(SubscriptionRecord s)
        {
            if (s == null) throw new ArgumentNullException("s");

            var i = Insert("core_subscription", s.Tenant)
                .InColumnValue("source", s.SourceId)
                .InColumnValue("action", s.ActionId)
                .InColumnValue("recipient", s.RecipientId)
                .InColumnValue("object", s.ObjectId ?? string.Empty)
                .InColumnValue("unsubscribed", !s.Subscribed);

            ExecNonQuery(i);
        }

        public void RemoveSubscriptions(int tenant, string sourceId, string actionId)
        {
            RemoveSubscriptions(tenant, sourceId, actionId, string.Empty);
        }

        public void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            var d = Delete("core_subscription", tenant).Where("source", sourceId).Where("action", actionId);
            if (objectId != string.Empty) d.Where("object", objectId ?? string.Empty);
            ExecNonQuery(d);
        }


        public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            var q = Query("core_subscriptionmethod", tenant)
                .Select("recipient", "sender")
                .Where("source", sourceId)
                .Where("action", actionId);
            if (recipientId != null) q.Where("recipient", recipientId);

            return ExecList(q)
                .ConvertAll(r =>
                {
                    return new SubscriptionMethod
                    {
                        Tenant = tenant,
                        SourceId = sourceId,
                        ActionId = actionId,
                        RecipientId = (string)r[0],
                        Methods = Convert.ToString(r[1]).Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                    };
                });
        }

        public void SetSubscriptionMethod(SubscriptionMethod m)
        {
            if (m == null) throw new ArgumentNullException("m");

            ISqlInstruction i = null;
            if (m.Methods == null || m.Methods.Length == 0)
            {
                i = Delete("core_subscriptionmethod", m.Tenant)
                    .Where("source", m.SourceId)
                    .Where("action", m.ActionId)
                    .Where("recipient", m.RecipientId);
            }
            else
            {
                i = Insert("core_subscriptionmethod", m.Tenant)
                    .InColumnValue("source", m.SourceId)
                    .InColumnValue("action", m.ActionId)
                    .InColumnValue("recipient", m.RecipientId)
                    .InColumnValue("sender", string.Join("|", m.Methods));
            }
            ExecNonQuery(i);
        }


        private SqlQuery GetQuery(int tenant, string sourceId, string actionId)
        {
            if (sourceId == null) throw new ArgumentNullException("sourceId");
            if (actionId == null) throw new ArgumentNullException("actionId");

            return Query("core_subscription", tenant)
                .Select("tenant", "source", "action", "recipient", "object", "unsubscribed")
                .Where("source", sourceId)
                .Where("action", actionId);
        }

        private SubscriptionRecord ToSubscriptionRecord(object[] r)
        {
            return new SubscriptionRecord
            {
                Tenant = Convert.ToInt32(r[0]),
                SourceId = (string)r[1],
                ActionId = (string)r[2],
                RecipientId = (string)r[3],
                ObjectId = string.Empty.Equals(r[4]) ? null : (string)r[4],
                Subscribed = !Convert.ToBoolean(r[5]),
            };
        }
    }
}
