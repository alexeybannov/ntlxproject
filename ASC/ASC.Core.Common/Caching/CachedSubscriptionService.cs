﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Core.Caching
{
    public class CachedSubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionService service;
        private readonly ICache cache;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }


        public CachedSubscriptionService(ISubscriptionService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();

            CacheExpiration = TimeSpan.FromMinutes(5);
        }


        public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId)
        {
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            return store.GetSubscriptions();
        }

        public IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId)
        {
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            return store.GetSubscriptions(recipientId, objectId);
        }

        public SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId)
        {
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            return store.GetSubscription(recipientId, objectId);
        }

        public void SaveSubscription(SubscriptionRecord s)
        {
            service.SaveSubscription(s);
            var store = GetSubsciptionsStore(s.Tenant, s.SourceId, s.ActionId);
            store.SaveSubscription(s);
        }

        public void RemoveSubscriptions(int tenant, string sourceId, string actionId)
        {
            service.RemoveSubscriptions(tenant, sourceId, actionId);
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            store.RemoveSubscriptions();
        }

        public void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId)
        {
            service.RemoveSubscriptions(tenant, sourceId, actionId, objectId);
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            store.RemoveSubscriptions(objectId);
        }


        public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId)
        {
            var store = GetSubsciptionsStore(tenant, sourceId, actionId);
            return store.GetSubscriptionMethods(recipientId);
        }

        public void SetSubscriptionMethod(SubscriptionMethod m)
        {
            service.SetSubscriptionMethod(m);
            var store = GetSubsciptionsStore(m.Tenant, m.SourceId, m.ActionId);
            store.SetSubscriptionMethod(m);
        }


        private SubsciptionsStore GetSubsciptionsStore(int tenant, string sourceId, string actionId)
        {
            var key = string.Format("sub/{0}/{1}/{2}", tenant, sourceId, actionId);
            var store = cache.Get(key) as SubsciptionsStore;
            if (store == null)
            {
                var records = service.GetSubscriptions(tenant, sourceId, actionId);
                var methods = service.GetSubscriptionMethods(tenant, sourceId, actionId, null);
                cache.Insert(key, store = new SubsciptionsStore(records, methods), CacheExpiration);
            }
            return store;
        }


        private class SubsciptionsStore
        {
            private readonly List<SubscriptionRecord> records;
            private IDictionary<string, List<SubscriptionRecord>> recordsByRec;
            private IDictionary<string, List<SubscriptionRecord>> recordsByObj;

            private readonly List<SubscriptionMethod> methods;
            private IDictionary<string, List<SubscriptionMethod>> methodsByRec;


            public SubsciptionsStore(IEnumerable<SubscriptionRecord> records, IEnumerable<SubscriptionMethod> methods)
            {
                this.records = records.ToList();
                this.methods = methods.ToList();
                BuildSubscriptionsIndex(records);
                BuildMethodsIndex(methods);
            }


            public IEnumerable<SubscriptionRecord> GetSubscriptions()
            {
                return records.ToList();
            }

            public IEnumerable<SubscriptionRecord> GetSubscriptions(string recipientId, string objectId)
            {
                return recipientId != null ?
                    recordsByRec.ContainsKey(recipientId) ? recordsByRec[recipientId].ToList() : new List<SubscriptionRecord>() :
                    recordsByObj.ContainsKey(objectId ?? string.Empty) ? recordsByObj[objectId ?? string.Empty].ToList() : new List<SubscriptionRecord>();
            }

            public SubscriptionRecord GetSubscription(string recipientId, string objectId)
            {
                return recordsByRec.ContainsKey(recipientId) ?
                    recordsByRec[recipientId].Where(s => s.ObjectId == objectId).FirstOrDefault() :
                    null;
            }

            public void SaveSubscription(SubscriptionRecord s)
            {
                var old = GetSubscription(s.RecipientId, s.ObjectId);
                if (old != null)
                {
                    old.Subscribed = s.Subscribed;
                }
                else
                {
                    records.Add(s);
                    BuildSubscriptionsIndex(records);
                }
            }

            public void RemoveSubscriptions()
            {
                records.Clear();
                BuildSubscriptionsIndex(records);
            }

            public void RemoveSubscriptions(string objectId)
            {
                records.RemoveAll(s => s.ObjectId == objectId);
                BuildSubscriptionsIndex(records);
            }


            public IEnumerable<SubscriptionMethod> GetSubscriptionMethods(string recipientId)
            {
                return string.IsNullOrEmpty(recipientId) ?
                    methods.ToList() :
                    methodsByRec.ContainsKey(recipientId) ? methodsByRec[recipientId].ToList() : new List<SubscriptionMethod>();
            }

            public void SetSubscriptionMethod(SubscriptionMethod m)
            {
                methods.RemoveAll(r => r.Tenant == m.Tenant && r.SourceId == m.SourceId && r.ActionId == m.ActionId && r.RecipientId == m.RecipientId);
                if (m.Methods != null && 0 < m.Methods.Length)
                {
                    methods.Add(m);
                }
                BuildMethodsIndex(methods);
            }


            private void BuildSubscriptionsIndex(IEnumerable<SubscriptionRecord> records)
            {
                recordsByRec = records.GroupBy(r => r.RecipientId).ToDictionary(g => g.Key, g => g.ToList());
                recordsByObj = records.GroupBy(r => r.ObjectId ?? string.Empty).ToDictionary(g => g.Key, g => g.ToList());
            }

            private void BuildMethodsIndex(IEnumerable<SubscriptionMethod> methods)
            {
                methodsByRec = methods.GroupBy(r => r.RecipientId).ToDictionary(g => g.Key, g => g.ToList());
            }
        }
    }
}
