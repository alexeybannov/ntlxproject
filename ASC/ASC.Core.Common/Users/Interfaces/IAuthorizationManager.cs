#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Users
{
    [Service("{0883BD87-12D6-49a3-9C3D-A5295B13AF4C}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    public interface IAuthorizationManager : IService
    {
        #region setting permissions

        AzRecord[] GetAces();

        AzRecord[] GetAces(Guid subjectID, Guid actionID);

        AzRecord[] GetAcesBySubject(Guid subjectID);

        AzRecord[] GetAcesByAction(Guid actionID);

        IList<AzObjectInfo> GetAzObjectInfos();

        AzObjectInfo GetAzObjectInfo(ISecurityObjectId objectId);

        AzObjectInfo GetAzObjectInfo<T>(object objectId);

        void SaveAzObjectInfo(AzObjectInfo azObjectInfo);

        void RemoveAzObjectInfo(AzObjectInfo azObjectInfo);

        AzRecord[] GetAces(Guid subjectID, Guid actionID, ISecurityObjectId objectId);

        AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId);

        AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId,
                                       ISecurityObjectProvider secObjProvider);

        AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObjectId objectId,
                                    ISecurityObjectProvider secObjProvider);

        AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObject securityObject);

        bool GetObjectAceInheritance(ISecurityObjectId objectId);

        void SetObjectAceInheritance(ISecurityObjectId objectId, bool inherit);

        bool GetObjectAceInheritance<T>(object objectId);

        void SetObjectAceInheritance<T>(object objectId, bool inherit);

        void AddAce(AzRecord azRecord);

        void RemoveAce(AzRecord azRecord);

        #endregion
    }
}