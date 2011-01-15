#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public interface IServicePublisher
    {
        ServiceLocatorRegistry ServiceLocatorRegistry { get; }

        HostLocatorRegistry HostLocatorRegistry { get; }

        ServiceInstanceIDResolverHolder ServiceInstanceIDResolver { get; }

        #region

        IService Publish(IServiceInfo srvInfo);

        IInstancedService Publish(IServiceInfo srvInfo, Guid serviceInstanceID);

        void Publish(IService service);

        void UnPublish(IService service);

        void UnPublish(IServiceInfo srvInfo);

        void UnPublish(IServiceInfo srvInfo, Guid serviceInstanceID);

        #endregion
    }
}