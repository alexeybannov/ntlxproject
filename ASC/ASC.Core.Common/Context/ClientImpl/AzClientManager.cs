using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;

namespace ASC.Core
{
    class AzClientManager : IAuthorizationManagerClient
    {
        private readonly IDictionary<int, IDictionary<string, AzRecord>> azAceCache = new Dictionary<int, IDictionary<string, AzRecord>>();
        private readonly IDictionary<int, IDictionary<Guid, List<string>>> azAceCacheByActions = new Dictionary<int, IDictionary<Guid, List<string>>>();

        private IDictionary<string, AzRecord> AzAceCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (azAceCache)
                {
                    if (!azAceCache.ContainsKey(tenant))
                    {
                        //azAceCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<string, AzRecord>(aceCacheInit, SyncAzAceCache);
                    }
                    return azAceCache[tenant];
                }
            }
        }

        private IDictionary<Guid, List<string>> AzAceCacheByActions
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (azAceCacheByActions)
                {
                    if (!azAceCacheByActions.ContainsKey(tenant))
                    {
                        azAceCacheByActions[tenant] = AzAceCache.Values.GroupBy(a => a.ActionId).ToDictionary(g => g.Key, g => g.Select(a => a.Id).ToList());
                    }
                    return azAceCacheByActions[tenant];
                }
            }
        }

        private IDictionary<string, AzRecord> SyncAzAceCache()
        {
            azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            //return CoreContext.InternalAuthorizationManager.GetAces().ToDictionary(a => a.Id);
            return null;
        }


        public AzRecord[] GetAces(Guid subjectID, Guid actionID)
        {
            return AzAceCache.Values.Where(a => a.SubjectId == subjectID && a.ActionId == actionID && a.FullObjectId == null).ToArray();
        }

        public AzRecord[] GetAces(Guid subjectID, Guid actionID, ISecurityObjectId objectId)
        {
            var fullObjectId = AzObjectIdHelper.GetFullObjectId(objectId);
            if (subjectID == Guid.Empty && actionID == Guid.Empty)
            {
                return AzAceCache.Values.Where(a => a.FullObjectId == fullObjectId).ToArray();
            }
            if (subjectID == Guid.Empty)
            {
                return AzAceCache.Values.Where(a => a.ActionId == actionID && a.FullObjectId == fullObjectId).ToArray();
            }
            if (actionID == Guid.Empty)
            {
                return AzAceCache.Values.Where(a => a.SubjectId == subjectID && a.FullObjectId == fullObjectId).ToArray();
            }
            return AzAceCache.Values.Where(a => a.SubjectId == subjectID && a.ActionId == actionID && a.FullObjectId == fullObjectId).ToArray();
        }

        public AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            if (actions == null) throw new ArgumentNullException("actions");
            if (objectId == null) throw new ArgumentNullException("objectId");

            //the code is not very intuitive, but it is fast
            var objectAces = new List<AzRecord>();
            var acesByActions = new List<AzRecord>();
            foreach (var id in actions.Select(a => a.ID))
            {
                List<string> azIds = null;
                if (AzAceCacheByActions.TryGetValue(id, out azIds))
                {
                    if (azIds != null)
                    {
                        acesByActions.AddRange(azIds.Select(azId => AzAceCache[azId]));
                    }
                }
            }
            var fullId = AzObjectIdHelper.GetFullObjectId(objectId);

            objectAces.AddRange(acesByActions.Where(a => a.FullObjectId == fullId));

            var inheritAces = new List<AzRecord>();
            var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, secObjProvider);
            while (secObjProviderHelper.NextInherit())
            {
                fullId = AzObjectIdHelper.GetFullObjectId(secObjProviderHelper.CurrentObjectId);
                inheritAces.AddRange(acesByActions.Where(a => a.FullObjectId == fullId));
            }

            inheritAces.AddRange(acesByActions.Where(a => a.FullObjectId == null));

            for (int i = 0; i < inheritAces.Count; i++)
            {
                inheritAces[i] = (AzRecord)inheritAces[i].Clone();
                inheritAces[i].Inherited = true;
            }
            objectAces.AddRange(DistinctAces(inheritAces));
            return objectAces.ToArray();
        }

        public void AddAce(AzRecord azRecord)
        {
            if (azRecord.Inherited) throw new InvalidOperationException("Can not add inherited authorization record");

            //CoreContext.InternalAuthorizationManager.AddAce(azRecord);
            if (!AzAceCache.ContainsKey(azRecord.Id))
            {
                azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            AzAceCache[azRecord.Id] = azRecord;
        }

        public void RemoveAce(AzRecord azRecord)
        {
            if (azRecord.Inherited) throw new InvalidOperationException("Can not remove inherited authorization record");

            //CoreContext.InternalAuthorizationManager.RemoveAce(azRecord);
            if (AzAceCache.ContainsKey(azRecord.Id))
            {
                azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            AzAceCache.Remove(azRecord.Id);
        }


        public AzObjectInfo GetAzObjectInfo(ISecurityObjectId objectId)
        {
            return null;
        }

        public void SaveAzObjectInfo(AzObjectInfo azObjectInfo)
        {
        }

        public void RemoveAzObjectInfo(AzObjectInfo azObjectInfo)
        {
        }


        private IEnumerable<AzRecord> DistinctAces(IEnumerable<AzRecord> inheritAces)
        {
            var aces = new Dictionary<string, AzRecord>();
            foreach (var a in inheritAces)
            {
                var key = string.Format("{0}{1}{2:D}", a.SubjectId, a.ActionId, a.Reaction);
                aces[string.Format("{0}{1}{2:D}", a.SubjectId, a.ActionId, a.Reaction)] = a;
            }
            return aces.Values;
        }
    }
}