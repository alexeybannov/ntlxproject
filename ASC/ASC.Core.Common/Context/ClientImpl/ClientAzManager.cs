using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core
{
    class ClientAzManager : IAuthManagerClient
    {
        private readonly IDbAzService service;


        public ClientAzManager(IDbAzService service)
        {
            this.service = service;
        }


        public AzRecord[] GetAces(Guid subjectId, Guid actionId)
        {
            return GetAcesInternal()
                .Where(a => a.SubjectId == subjectId && a.ActionId == actionId && a.ObjectId == null)
                .ToArray();
        }

        public AzRecord[] GetAces(Guid subjectID, Guid actionID, ISecurityObjectId objectId)
        {
            var fullObjectId = AzObjectIdHelper.GetFullObjectId(objectId);
            if (subjectID == Guid.Empty && actionID == Guid.Empty)
            {
                return GetAcesInternal()
                    .Where(a => a.ObjectId == fullObjectId).ToArray();
            }
            if (subjectID == Guid.Empty)
            {
                return GetAcesInternal()
                    .Where(a => a.ActionId == actionID && a.ObjectId == fullObjectId).ToArray();
            }
            if (actionID == Guid.Empty)
            {
                return GetAcesInternal()
                    .Where(a => a.SubjectId == subjectID && a.ObjectId == fullObjectId).ToArray();
            }
            return GetAcesInternal()
                .Where(a => a.SubjectId == subjectID && a.ActionId == actionID && a.ObjectId == fullObjectId).ToArray();
        }

        public AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            if (actions == null) throw new ArgumentNullException("actions");
            if (objectId == null) throw new ArgumentNullException("objectId");

            var fullId = AzObjectIdHelper.GetFullObjectId(objectId);
            var actionIds = actions.Select(a => a.ID);
            var objectAces = new List<AzRecord>();
            var acesByActions = GetAcesInternal()
                .Where(a => actionIds.Contains(a.ActionId))
                .ToList();

            objectAces.AddRange(acesByActions.Where(a => a.ObjectId == fullId));

            var inheritAces = new List<AzRecord>();
            var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, secObjProvider);
            while (secObjProviderHelper.NextInherit())
            {
                fullId = AzObjectIdHelper.GetFullObjectId(secObjProviderHelper.CurrentObjectId);
                inheritAces.AddRange(acesByActions.Where(a => a.ObjectId == fullId));
            }

            inheritAces.AddRange(acesByActions.Where(a => a.ObjectId == null));

            objectAces.AddRange(DistinctAces(inheritAces));
            return objectAces.ToArray();
        }

        public void AddAce(AzRecord r)
        {
            service.SaveAce(CoreContext.TenantManager.GetCurrentTenant().TenantId, r);
        }

        public void RemoveAce(AzRecord r)
        {
            service.RemoveAce(CoreContext.TenantManager.GetCurrentTenant().TenantId, r);
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


        private IEnumerable<AzRecord> GetAcesInternal()
        {
            return service.GetAces(CoreContext.TenantManager.GetCurrentTenant().TenantId, default(DateTime));
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