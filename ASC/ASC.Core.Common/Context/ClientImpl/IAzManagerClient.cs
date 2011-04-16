using System;
using System.Collections.Generic;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core
{
    public interface IAzManagerClient
    {
        AzRecord[] GetAces(Guid subjectID, Guid actionID);

        AzRecord[] GetAces(Guid subjectID, Guid actionID, ISecurityObjectId objectId);

        AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider);

        void AddAce(AzRecord azRecord);

        void RemoveAce(AzRecord azRecord);


        AzObjectInfo GetAzObjectInfo(ISecurityObjectId objectId);

        void SaveAzObjectInfo(AzObjectInfo azObjectInfo);

        void RemoveAzObjectInfo(AzObjectInfo azObjectInfo);
    }
}