using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Cache;
using ASC.Core.Users;
using UserConst = ASC.Core.Users.Constants;

namespace ASC.Core
{
    class AzClientManager : IAuthorizationManagerClient
    {
        private readonly IDictionary<int, ICache<string, AzRecord>> azAceCache = new Dictionary<int, ICache<string, AzRecord>>();
        private readonly IDictionary<int, ICache<string, AzObjectInfo>> azObjectInfoCache = new Dictionary<int, ICache<string, AzObjectInfo>>();
        private readonly IDictionary<int, IDictionary<Guid, List<string>>> azAceCacheByActions = new Dictionary<int, IDictionary<Guid, List<string>>>();

        private ICache<string, AzRecord> AzAceCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (azAceCache)
                {
                    if (!azAceCache.ContainsKey(tenant))
                    {
                        var aceCacheInit = new CacheInfo(UserConst.CacheIdAzAce + tenant);
                        aceCacheInit.AddParentCache(UserConst.CacheIdUsers + tenant);
                        aceCacheInit.AddParentCache(UserConst.CacheIdCategories + tenant);
                        aceCacheInit.AddParentCache(UserConst.CacheIdGroups + tenant);
                        azAceCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<string, AzRecord>(aceCacheInit, SyncAzAceCache);
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

        private ICache<string, AzObjectInfo> AzObjectInfoCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (azObjectInfoCache)
                {
                    if (!azObjectInfoCache.ContainsKey(tenant))
                    {
                        var azObjectInfoCacheInit = new CacheInfo(UserConst.CacheIdAzAce + tenant);
                        azObjectInfoCacheInit.AddParentCache(UserConst.CacheIdUsers + tenant);
                        azObjectInfoCacheInit.AddParentCache(UserConst.CacheIdCategories + tenant);
                        azObjectInfoCacheInit.AddParentCache(UserConst.CacheIdGroups + tenant);
                        azObjectInfoCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<string, AzObjectInfo>(azObjectInfoCacheInit, SyncAzObjectInfoCache);
                    }
                    return azObjectInfoCache[tenant];
                }
            }
        }

        private IDictionary<string, AzRecord> SyncAzAceCache()
        {
            azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            return CoreContext.InternalAuthorizationManager.GetAces().ToDictionary(a => a.Id);
        }

        private IDictionary<string, AzObjectInfo> SyncAzObjectInfoCache()
        {
            return CoreContext.InternalAuthorizationManager.GetAzObjectInfos().ToDictionary(a => a.ObjectId);
        }

        #region IAuthorizationManager Members

        public AzRecord[] GetAces()
        {
            return AzAceCache.Values.ToArray();
        }

        public AzRecord[] GetAces(Guid subjectID, Guid actionID)
        {
            return AzAceCache.Values.Where(a => a.SubjectId == subjectID && a.ActionId == actionID && a.FullObjectId == null).ToArray();
        }

        public AzRecord[] GetAcesBySubject(Guid subjectID)
        {
            return AzAceCache.Values.Where(a => a.SubjectId == subjectID && a.FullObjectId == null).ToArray();
        }

        public AzRecord[] GetAcesByAction(Guid actionID)
        {
            return AzAceCache.Values.Where(a => a.ActionId == actionID && a.FullObjectId == null).ToArray();
        }

        public IList<AzObjectInfo> GetAzObjectInfos()
        {
            return AzObjectInfoCache.Values.ToList();
        }

        public AzObjectInfo GetAzObjectInfo<T>(object objectId)
        {
            return GetAzObjectInfo(new SecurityObjectId<T>(objectId));
        }

        public AzObjectInfo GetAzObjectInfo(ISecurityObjectId objectId)
        {
            var fullId = AzObjectIdHelper.GetFullObjectId(objectId);
            return AzObjectInfoCache.ContainsKey(fullId) ? AzObjectInfoCache[fullId] : new AzObjectInfo(objectId);
        }

        public void SaveAzObjectInfo(AzObjectInfo azObjectInfo)
        {
            CoreContext.InternalAuthorizationManager.SaveAzObjectInfo(azObjectInfo);
            AzObjectInfoCache[azObjectInfo.ObjectId] = azObjectInfo;
        }

        public void RemoveAzObjectInfo(AzObjectInfo azObjectInfo)
        {
            CoreContext.InternalAuthorizationManager.RemoveAzObjectInfo(azObjectInfo);
            AzObjectInfoCache.Remove(azObjectInfo.ObjectId);
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

        public AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId)
        {
            return GetAllObjectAces<T>(actions, objectId, null);
        }

        public AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId, ISecurityObjectProvider secObjProvider)
        {
            return GetAllObjectAces(actions, new SecurityObjectId<T>(objectId), secObjProvider);
        }

        public AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObject securityObject)
        {
            return GetAllObjectAces(actions, securityObject, null);
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
            var isInheritAces = GetObjectAceInheritance(objectId);

            if (isInheritAces)
            {
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
            }
            return objectAces.ToArray();
        }

        public bool GetObjectAceInheritance(ISecurityObjectId objectId)
        {
            if (objectId == null) throw new ArgumentNullException("objectId");
            return GetAzObjectInfo(objectId).InheritAces;
        }

        public bool GetObjectAceInheritance<T>(object objectId)
        {
            return GetObjectAceInheritance(new SecurityObjectId<T>(objectId));
        }

        public void SetObjectAceInheritance(ISecurityObjectId objectId, bool inherit)
        {
            if (objectId == null) throw new ArgumentNullException("objectId");
            SecurityContext.DemandPermissions(UserConst.Action_EditAz);
            AzObjectInfo azObjectInfo = GetAzObjectInfo(objectId);
            azObjectInfo.InheritAces = inherit;
            SaveAzObjectInfo(azObjectInfo);
        }

        public void SetObjectAceInheritance<T>(object objectId, bool inherit)
        {
            SetObjectAceInheritance(new SecurityObjectId<T>(objectId), inherit);
        }

        public void AddAce(AzRecord azRecord)
        {
            CoreContext.InternalAuthorizationManager.AddAce(azRecord);
            if (!AzAceCache.ContainsKey(azRecord.Id))
            {
                azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            AzAceCache[azRecord.Id] = azRecord;
        }

        public void RemoveAce(AzRecord azRecord)
        {
            CoreContext.InternalAuthorizationManager.RemoveAce(azRecord);
            if (AzAceCache.ContainsKey(azRecord.Id))
            {
                azAceCacheByActions.Remove(CoreContext.TenantManager.GetCurrentTenant().TenantId);
            }
            AzAceCache.Remove(azRecord.Id);
        }

        #endregion

        private List<AzRecord> GetAcesByObjectId(string fullObjId)
        {
            return AzAceCache.Values.Where(a => a.FullObjectId == fullObjId).ToList();
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