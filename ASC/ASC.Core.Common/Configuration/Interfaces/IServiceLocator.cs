#region usings

using System;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Net;

#endregion

namespace ASC.Core.Configuration
{
    [Service("{AC9037AE-800F-4fc9-841A-D5179A351701}", ServiceInstancingType.Singleton)]
    [AuthenticationLevel(SecurityLevel.SysService)]
    public interface IServiceLocator : IService
    {
        ServiceHostingModel GetServiceHostingModel(Guid serviceID);

        IServiceInfo GetServiceInfo(Guid serviceId);

        void ServiceStartedUp(IServiceInfo serviceInfo, Guid serviceInstanceID, ConnectionHostEntry hostEntry);

        void ServiceShutDowned(Guid serviceID, Guid serviceInstanceID);

        #region resolving location

        [AuthenticationLevel(SecurityLevel.SessionTicket)]
        ConnectionHostEntry ResolveLocation(Guid serviceID, Guid serviceInstanceID);

        [AuthenticationLevel(SecurityLevel.SessionTicket)]
        ServiceLocation[] InstancedServicesDislocation(Guid serviceID);

        #endregion
    }
}