#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public class AzObjectSecurityProviderHelper
    {
        private readonly SecurityCallContext callContext;
        private readonly bool currObjIdAsProvider;
        private ISecurityObjectId currObjId;
        private ISecurityObjectProvider currSecObjProvider;

        public AzObjectSecurityProviderHelper(ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            if (objectId == null) throw new ArgumentNullException("objectId");
            currObjIdAsProvider = false;
            currObjId = objectId;
            currSecObjProvider = secObjProvider;
            if (currSecObjProvider == null && currObjId is ISecurityObjectProvider)
            {
                currObjIdAsProvider = true;
                currSecObjProvider = (ISecurityObjectProvider) currObjId;
            }
            callContext = new SecurityCallContext();
        }

        public ISecurityObjectId CurrentObjectId
        {
            get { return currObjId; }
        }

        public bool ObjectRolesSupported
        {
            get { return currSecObjProvider != null && currSecObjProvider.ObjectRolesSupported; }
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account)
        {
            IEnumerable<IRole> roles = currSecObjProvider.GetObjectRoles(account, currObjId, callContext);
            foreach (IRole role in roles)
            {
                if (!callContext.RolesList.Contains(role)) callContext.RolesList.Add(role);
            }
            return roles;
        }

        public bool NextInherit()
        {
            if (currSecObjProvider == null || !currSecObjProvider.InheritSupported) return false;
            currObjId = currSecObjProvider.InheritFrom(currObjId);
            if (currObjId == null) return false;
            if (currObjIdAsProvider)
            {
                currSecObjProvider = currObjId as ISecurityObjectProvider;
            }
            callContext.ObjectsStack.Insert(0, CurrentObjectId);
            return currSecObjProvider != null;
        }
    }
}