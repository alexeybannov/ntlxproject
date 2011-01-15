#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public interface IServiceHost : IController
    {
        string Name { get; }

        HostStatus Status { get; }
        IServiceInfo[] Services { get; }

        void Start(Guid serviceID);

        void Stop(Guid serviceID);

        ServiceStatus GetStatus(Guid serviceID);

        void Add(IServiceController srvController);

        void Add(IServiceInfo srvInfo);

        void Remove(IServiceInfo srvInfo);

        #region

        event ServiceUnhandledExceptionEventHandler UnhandledException;

        event ServiceStatusChangeEventEventHandler StatusChange;

        event HostStatusChangeEventEventHandler HostStatusChange;

        #endregion
    }
}