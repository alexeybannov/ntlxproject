#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public delegate void ServiceStatusChangeEventEventHandler(object sender, ServiceStatusChangeEventArgs e);

    [Serializable]
    public class ServiceStatusChangeEventArgs : ServiceEventsEventArgs
    {
        private readonly ServiceStatus _NewStatus = ServiceStatus.Unknown;
        private readonly ServiceStatus _PreviousStatus = ServiceStatus.Unknown;

        public ServiceStatusChangeEventArgs(IServiceInfo srvInfo, ServiceStatus previousStatus, ServiceStatus newStatus)
            :
                base(srvInfo)
        {
            _PreviousStatus = previousStatus;
            _NewStatus = newStatus;
        }

        public ServiceStatus PreviousStatus
        {
            get { return _PreviousStatus; }
        }

        public ServiceStatus NewStatus
        {
            get { return _NewStatus; }
        }
    }
}