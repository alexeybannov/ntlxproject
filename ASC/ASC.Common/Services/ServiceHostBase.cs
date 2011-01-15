#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Module;
using ASC.Reflection;
using log4net;

#endregion

namespace ASC.Common.Services
{
    public class ServiceHostBase
        : MarshalByRefObject, IServiceHost
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (ServiceHostBase));

        #region

        public ServiceHostBase()
            : this("<unknown>")
        {
        }

        public ServiceHostBase(string name)
        {
            if (!String.IsNullOrEmpty(name))
                _Name = name;
        }

        public ServiceHostBase(IModuleServicesPart servicePart)
            : this(null, servicePart)
        {
        }

        public ServiceHostBase(string name, IModuleServicesPart servicePart)
            : this(name)
        {
            if (servicePart == null)
                throw new ArgumentNullException("servicePart");

            _ServicePart = servicePart;
            _SnapshotServices();
        }

        #endregion

        #region

        protected IServiceController this[Guid serviceID]
        {
            get
            {
                if (!_ServicesList.ContainsKey(serviceID))
                    throw new ServiceNotFoundException(serviceID.ToString());
                return _ServicesList[serviceID];
            }
        }

        #endregion

        private readonly string _Name = "<unknown>";
        private readonly IModuleServicesPart _ServicePart;

        private readonly IDictionary<Guid, IServiceController> _ServicesList =
            new Dictionary<Guid, IServiceController>(10);

        private HostStatus _Status = HostStatus.Unknown;

        #region IServiceHost Members

        public void Start()
        {
            log.DebugFormat("starting host \"{0}\"...", _Name);

            BeforeStart();
            StartInternal();
            AfterStart();
        }

        public void Stop()
        {
            log.DebugFormat("stopping host \"{0}\"...", _Name);

            BeforeStop();
            StopInternal();
            AfterStop();
        }

        public string Name
        {
            get { return _Name; }
        }

        public HostStatus Status
        {
            get { return _Status; }
            protected set
            {
                if (_Status == value) return;

                HostStatus prevStatus = _Status;
                _Status = value;

                if (HostStatusChange != null)
                    HostStatusChange(this, new HostStatusChangeEventArgs(Name, prevStatus, _Status));
            }
        }

        public void Start(Guid serviceID)
        {
            BeforeStart(serviceID);
            StartInternal(serviceID);
            AfterStart(serviceID);
        }

        public void Stop(Guid serviceID)
        {
            BeforeStop(serviceID);
            StopInternal(serviceID);
            AfterStop(serviceID);
        }

        public ServiceStatus GetStatus(Guid serviceID)
        {
            return this[serviceID].Status;
        }

        public IServiceInfo[] Services
        {
            get
            {
                var infos = new IServiceInfo[_ServicesList.Count];
                int i = 0;
                foreach (IServiceController srvController in _ServicesList.Values)
                    infos[i++] = srvController.Info;
                return infos;
            }
        }

        public void Add(IServiceController srvController)
        {
            if (srvController == null)
                throw new ArgumentNullException("srvController");
            if (srvController.Info == null)
                throw new ArgumentNullException("srvController.Info");
            if (srvController.Info.InstancingType != ServiceInstancingType.Singleton)
            {
                if (!TypeHelper.ImplementInterface(srvController.GetType(), typeof (IInstancedService)))
                    throw new InstancingTypeMismatchException(srvController.Info.ToString(),
                                                              "service instance is instanced service type but dosn't implement IInstancedService");
                if ((srvController as IInstancedService).InstanceID == ServiceInfoBase.DefaultInstanceID)
                    throw new InstancingTypeMismatchException(srvController.Info.ToString(),
                                                              "instanced service must has not default instanceID");
            }
            AddService(srvController);
        }

        public void Add(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");

            AddService(srvInfo);
        }

        public void Remove(IServiceInfo srvInfo)
        {
            RemoveService(srvInfo);
        }

        public event ServiceUnhandledExceptionEventHandler UnhandledException;

        public event ServiceStatusChangeEventEventHandler StatusChange;

        public event HostStatusChangeEventEventHandler HostStatusChange;

        #endregion

        #region

        private void _SnapshotServices()
        {
            if (_ServicePart == null)
                return;
            foreach (IServiceInfo srvInfo in _ServicePart.Services)
                Add(srvInfo);
        }

        private void _CalculateHostStatus()
        {
            HostStatus newStatus = HostStatus.Unknown;
            bool oneRunning = false;
            bool oneWarning = false;
            bool oneError = false;
            newStatus = HostStatus.RunningAll | HostStatus.WarningAll | HostStatus.ErrorAll;
            foreach (IServiceController srvContr in _ServicesList.Values)
            {
                if ((srvContr.Status & ServiceStatus.Running) == ServiceStatus.Unknown)
                    newStatus = newStatus & (HostStatus) 0xFF0 | HostStatus.RunningSome;
                else
                    oneRunning = true;

                if ((srvContr.Status & ServiceStatus.Warning) == ServiceStatus.Unknown)
                    newStatus = newStatus & (HostStatus) 0xF0F | HostStatus.WarningSome;
                else
                    oneWarning = true;

                if ((srvContr.Status & ServiceStatus.Error) == ServiceStatus.Unknown)
                    newStatus = newStatus & (HostStatus) 0x0FF | HostStatus.ErrorSome;
                else
                    oneError = true;
            }
            if (!oneRunning) newStatus = newStatus & (HostStatus) 0xFF0 | HostStatus.Stopped;
            if (!oneWarning) newStatus = newStatus & (HostStatus) 0xF0F;
            if (!oneError) newStatus = newStatus & (HostStatus) 0x0FF;
            Status = newStatus;
        }

        #endregion

        #region

        protected virtual IServiceController CreateService(IServiceInfo srvInfo)
        {
            return null;
        }

        protected virtual IServiceController AddService(IServiceInfo srvInfo)
        {
            log.DebugFormat("adding service \"{0}\" at host \"{1}\"...", srvInfo.Name, _Name);
            IServiceController srvController = CreateService(srvInfo);

            if (srvController == null && _ServicePart != null)
                srvController = _ServicePart.CreateServiceInstance(srvInfo);
            if (srvController == null)
            {
                throw new ServiceInstancingException(srvInfo.Name);
            }
            AddService(srvController);
            return srvController;
        }

        protected virtual void RemoveService(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");

            Stop(srvInfo.ID);

            this[srvInfo.ID].StatusChange -= OnServiceStatusChange;
            this[srvInfo.ID].UnhandledException -= OnServiceException;

            _ServicesList.Remove(srvInfo.ID);
        }

        protected virtual void AddService(IServiceController srvController)
        {
            _ServicesList.Add(srvController.Info.ID, srvController);

            srvController.StatusChange += OnServiceStatusChange;
            srvController.UnhandledException += OnServiceException;
        }

        protected virtual void RemoveService(IServiceController srvController)
        {
            if (srvController == null)
                throw new ArgumentNullException("srvInfo");
            RemoveService(srvController.Info);
        }

        protected virtual void BeforeStart()
        {
        }

        protected virtual void StartInternal()
        {
            foreach (IServiceInfo srvInfo in Services)
                Start(srvInfo.ID);
        }

        protected virtual void AfterStart()
        {
        }

        protected virtual void BeforeStop()
        {
        }

        protected virtual void StopInternal()
        {
            foreach (IServiceInfo srvInfo in Services)
                Stop(srvInfo.ID);
        }

        protected virtual void AfterStop()
        {
        }

        protected virtual void BeforeStart(Guid serviceID)
        {
        }

        protected virtual void StartInternal(Guid serviceID)
        {
            this[serviceID].Start();
        }

        protected virtual void AfterStart(Guid serviceID)
        {
        }

        protected virtual void BeforeStop(Guid serviceID)
        {
        }

        protected virtual void StopInternal(Guid serviceID)
        {
            this[serviceID].Stop();
        }

        protected virtual void AfterStop(Guid serviceID)
        {
        }

        protected virtual void OnServiceException(object sender, ServiceExceptionEventArgs e)
        {
            var service = sender as IServiceController;
            if (service == null) return;

            if (e.Guilty == null)
                e.Guilty = service;
            if (UnhandledException != null)
                UnhandledException(service.Info, e);
        }

        protected virtual void OnServiceStatusChange(object sender, ServiceStatusChangeEventArgs e)
        {
            var service = sender as IServiceController;
            if (service == null) return;
            _CalculateHostStatus();

            if (StatusChange != null)
                StatusChange(service.Info, e);
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}