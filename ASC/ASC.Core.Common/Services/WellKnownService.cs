#region usings

using System;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Common.Services
{
    public abstract class WellKnownService
        : WellKnownServiceBase
    {
        protected WellKnownService(IServiceInfo srvInfo)
            : base(
                srvInfo,
                WorkContext.ServicePublisher.ServiceInstanceIDResolver.Resolve(srvInfo)
                )
        {
        }

        protected WellKnownService(IServiceInfo srvInfo, Guid serviceInstanceID)
            : base(srvInfo, serviceInstanceID)
        {
        }
    }
}