#region usings

using System;
using log4net;

#endregion

namespace ASC.Common.Services
{
    public abstract class ServiceControllerBase
        : MarshalByRefObject, IServiceController, IDisposable, IInstancedService
    {
        #region

        private static readonly ILog log = LogManager.GetLogger(typeof (ServiceControllerBase));
        private readonly Guid _ServiceInstanceID = ServiceInfoBase.DefaultInstanceID;
        private IServiceInfo _ServiceInfo;
        private ServiceStatus _Status = ServiceStatus.Stopped | ServiceStatus.Ok;

        #endregion

        protected ServiceControllerBase(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            if (srvInfo.InstancingType == ServiceInstancingType.MultipleInstances ||
                srvInfo.InstancingType == ServiceInstancingType.MultipleFixedInstance)
                throw new InstancingTypeMismatchException(srvInfo.Name,
                                                          "must using ServiceControllerBase(IServiceInfo srvInfo, Guid serviceInstanceID) constructor");
            _ServiceInfo = srvInfo;
        }

        protected ServiceControllerBase(IServiceInfo srvInfo, Guid serviceInstanceID)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton &&
                serviceInstanceID != ServiceInfoBase.DefaultInstanceID)
                throw new InstancingTypeMismatchException(srvInfo.Name,
                                                          "must using ServiceControllerBase(IServiceInfo srvInfo) constructor");
            if (srvInfo.InstancingType != ServiceInstancingType.Singleton &&
                serviceInstanceID == ServiceInfoBase.DefaultInstanceID)
                throw new ArgumentOutOfRangeException("serviceInstanceID");
            _ServiceInfo = srvInfo;
            _ServiceInstanceID = serviceInstanceID;
        }

        #region IServiceController Members

        public void Start()
        {
            log.DebugFormat("starting service \"{0}\"...", Info.Name);

            Status = Status & ServiceStatus.Sys_FF00 | ServiceStatus.StartPendind;
            try
            {
                StartInternal();

                Status = Status & ServiceStatus.Sys_FF00 | ServiceStatus.Running;
            }
            catch (Exception exc)
            {
                Status = Status & ServiceStatus.Sys_FFFF0000 | ServiceStatus.Stopped | ServiceStatus.Error;

                FireEvent(new ServiceExceptionEventArgs(Info, exc));
            }
        }

        public void Stop()
        {
            log.DebugFormat("stopping service \"{0}\"...", Info.Name);

            Status = Status & ServiceStatus.Sys_FF00 | ServiceStatus.StopPendind;
            try
            {
                StopInternal();

                Status = Status & ServiceStatus.Sys_FFFF0000 | ServiceStatus.Stopped | ServiceStatus.Ok;
            }
            catch (Exception exc)
            {
                Status = Status & ServiceStatus.Sys_FFFF0000 | ServiceStatus.Stopped | ServiceStatus.Error;

                FireEvent(new ServiceExceptionEventArgs(Info, exc));
            }
        }

        public IServiceInfo Info
        {
            get { return _ServiceInfo; }
            protected set
            {
                if (value == null) throw new ArgumentNullException();
                _ServiceInfo = value;
            }
        }

        public ServiceStatus Status
        {
            get { return _Status; }
            set
            {
                if (_Status == value) return;

                ServiceStatus prevStatus = _Status;
                _Status = value;

                FireEvent(new ServiceStatusChangeEventArgs(Info, prevStatus, _Status));
            }
        }

        public event ServiceUnhandledExceptionEventHandler UnhandledException;

        public event ServiceStatusChangeEventEventHandler StatusChange;

        #endregion

        #region

        protected abstract void StartInternal();

        protected abstract void StopInternal();

        protected void FireEvent(ServiceExceptionEventArgs e)
        {
            FireEvent(this, e);
        }

        protected void FireEvent(object sender, ServiceExceptionEventArgs e)
        {
            if (UnhandledException != null)
                UnhandledException(sender, e);
        }

        protected void FireEvent(ServiceStatusChangeEventArgs e)
        {
            FireEvent(this, e);
        }

        protected void FireEvent(object sender, ServiceStatusChangeEventArgs e)
        {
            if (StatusChange != null)
                StatusChange(sender, e);
        }

        #endregion

        #region IDisposable

        private bool _Disposed;

        public void Dispose()
        {
            log.InfoFormat("disposing service \"{0}\"...", Info.Name);

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_Disposed)
            {
                if (disposing)
                {
                    Stop();
                }
            }
            _Disposed = true;
        }

        ~ServiceControllerBase()
        {
            Dispose(false);
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IInstanced

        public Guid InstanceID
        {
            get { return _ServiceInstanceID; }
        }

        #endregion
    }
}