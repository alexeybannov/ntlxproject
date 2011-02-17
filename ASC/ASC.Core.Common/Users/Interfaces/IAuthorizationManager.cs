using System.Collections.Generic;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

namespace ASC.Core.Users
{
    [Service("{0883BD87-12D6-49a3-9C3D-A5295B13AF4C}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    interface IAuthorizationManager : IService
    {
        AzRecord[] GetAces();

        void AddAce(AzRecord azRecord);

        void RemoveAce(AzRecord azRecord);

        
        IList<AzObjectInfo> GetAzObjectInfos();

        void SaveAzObjectInfo(AzObjectInfo azObjectInfo);

        void RemoveAzObjectInfo(AzObjectInfo azObjectInfo);
    }
}