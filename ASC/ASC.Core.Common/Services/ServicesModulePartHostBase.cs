#region usings

using System;
using System.Collections.Generic;
using System.Reflection;
using ASC.Common.Module;
using ASC.Common.Services;
using SmartAssembly.Attributes;

#endregion

namespace ASC.Core.Common.Services
{
    public abstract class ServicesModulePartHostBase
        : ServiceController, IServicesModulePartsHost, IService
    {
        private HostStatus _Status = HostStatus.Unknown;
        private readonly IDictionary<Guid, SMPEntry> _ModulePartsList = new Dictionary<Guid, SMPEntry>();

        public ServicesModulePartHostBase(IServiceInfo srvInfo)
            : base(srvInfo)
        {
        }

        public ServicesModulePartHostBase(IServiceInfo srvInfo, Guid instanceID)
            : base(srvInfo, instanceID)
        {
        }

        #region internal types

        protected class SMPEntry
        {
            public SMPEntry(Guid servicesModulePartID)
            {
                ServiceModulePartID = servicesModulePartID;
            }

            public readonly Guid ServiceModulePartID;

            public IModulePartInfo ServiceModulePartInfo;

            public IModuleServicesPart ModuleServicesPart;

            public IServiceHost ModulePartHost;

            public bool InfoRetrived
            {
                get { return ServiceModulePartInfo != null; }
            }

            public bool PartInstanced
            {
                get { return ModuleServicesPart != null; }
            }

            public bool HostInstanced
            {
                get { return ModulePartHost != null; }
            }

            public object Tag;
        }

        #endregion

        #region IController

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IController.Start()
        {
            ControllerStart();
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        public void ControllerStart()
        {
            foreach (Guid id in _ModulePartsList.Keys)
                (this as IServicesModulePartsHost).Start(id);
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IController.Stop()
        {
            foreach (Guid id in _ModulePartsList.Keys)
                (this as IServicesModulePartsHost).Stop(id);
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        public void ControllerStop()
        {
            foreach (Guid id in _ModulePartsList.Keys)
                (this as IServicesModulePartsHost).Stop(id);
        }

        #endregion

        #region IServicesModulePartsHost

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IServicesModulePartsHost.Start(Guid servicesModulePartID)
        {
            if (_ModulePartsList.ContainsKey(servicesModulePartID))
            {
                BeforeStartPart(servicesModulePartID);
                StartPart(servicesModulePartID);
                AfterStartPart(servicesModulePartID);
            }
            else
                throw new ModulePartNotFoundException(servicesModulePartID);
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IServicesModulePartsHost.Stop(Guid servicesModulePartID)
        {
            if (_ModulePartsList.ContainsKey(servicesModulePartID))
            {
                BeforeStopPart(servicesModulePartID);
                StopPart(servicesModulePartID);
                AfterStopPart(servicesModulePartID);
            }
            else
                throw new ModulePartNotFoundException(servicesModulePartID);
        }

        Guid[] IServicesModulePartsHost.ServicesModuleParts
        {
            [Obfuscation(Exclude = true)]
            [DoNotObfuscate]
            get
            {
                var result = new Guid[_ModulePartsList.Count];
                _ModulePartsList.Keys.CopyTo(result, 0);
                return result;
            }
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IServicesModulePartsHost.Add(Guid servicesModulePartID)
        {
            if (!_ModulePartsList.ContainsKey(servicesModulePartID))
            {
                BeforeAddPart(servicesModulePartID);
                AddPart(servicesModulePartID);
                AfterAddPart(servicesModulePartID);
            }
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        void IServicesModulePartsHost.Remove(Guid servicesModulePartID)
        {
            if (_ModulePartsList.ContainsKey(servicesModulePartID))
            {
                (this as IServicesModulePartsHost).Stop(servicesModulePartID);

                BeforeRemovePart(servicesModulePartID);
                RemovePart(servicesModulePartID);
                AfterRemovePart(servicesModulePartID);
            }
            else
                throw new ModulePartNotFoundException(servicesModulePartID);
        }

        HostStatus IServicesModulePartsHost.Status
        {
            [Obfuscation(Exclude = true)]
            [DoNotObfuscate]
            get { return _Status; }
        }

        [Obfuscation(Exclude = true)]
        [DoNotObfuscate]
        HostStatus IServicesModulePartsHost.GetStatus(Guid servicesModulePartID)
        {
            if (!_ModulePartsList.ContainsKey(servicesModulePartID))
                throw new ModulePartNotFoundException(servicesModulePartID);
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            if (entry.HostInstanced)
                return entry.ModulePartHost.Status;
            else
                return HostStatus.Unknown;
        }

        #endregion

        protected SMPEntry this[Guid servicesModulePartID]
        {
            get
            {
                if (!_ModulePartsList.ContainsKey(servicesModulePartID))
                    throw new ModulePartNotFoundException(servicesModulePartID);
                return _ModulePartsList[servicesModulePartID];
            }
        }

        #region virtual methods of behavior

        #region appendix/removal

        protected virtual void BeforeAddPart(Guid servicesModulePartID)
        {
        }

        protected virtual void AddPart(Guid servicesModulePartID)
        {
            _ModulePartsList.Add(servicesModulePartID, new SMPEntry(servicesModulePartID));
        }

        protected virtual void AfterAddPart(Guid servicesModulePartID)
        {
        }

        protected virtual void BeforeRemovePart(Guid servicesModulePartID)
        {
        }

        protected virtual void RemovePart(Guid servicesModulePartID)
        {
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            if (entry.HostInstanced)
            {
                entry.ModulePartHost.StatusChange -= OnServiceStatusChange;
                entry.ModulePartHost.UnhandledException -= OnServiceException;
                entry.ModulePartHost.HostStatusChange -= OnHostStatusChange;
            }
            _ModulePartsList.Remove(servicesModulePartID);
        }

        protected virtual void AfterRemovePart(Guid servicesModulePartID)
        {
        }

        #endregion

        #region start/stop

        protected virtual void BeforeStartPart(Guid servicesModulePartID)
        {
        }

        protected virtual void StartPart(Guid servicesModulePartID)
        {
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            if (!entry.HostInstanced)
            {
                _FillModulePartHostInstance(servicesModulePartID);
                if (!entry.HostInstanced)
                    return;
            }

            HostStatus status = entry.ModulePartHost.Status;
            if ((status & HostStatus.RunningAll) != HostStatus.RunningAll)
            {
                entry.ModulePartHost.Start();
            }
        }

        protected virtual void AfterStartPart(Guid servicesModulePartID)
        {
        }

        protected virtual void BeforeStopPart(Guid servicesModulePartID)
        {
        }

        protected virtual void StopPart(Guid servicesModulePartID)
        {
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            if (entry.HostInstanced)
            {
                HostStatus status = entry.ModulePartHost.Status;
                if ((status & HostStatus.Stopped) != HostStatus.Stopped)
                {
                    entry.ModulePartHost.Stop();
                }
            }
        }

        protected virtual void AfterStopPart(Guid servicesModulePartID)
        {
        }

        #endregion

        #region obtain information about the modules

        protected abstract void FillModulePartInfo(Guid servicesModulePartID);

        protected abstract void FillModulePartInstance(Guid servicesModulePartID);

        protected virtual void FillModulePartHostInstance(Guid servicesModulePartID)
        {
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            if (entry.HostInstanced) return;
            if (!entry.PartInstanced)
                FillModulePartInstance(servicesModulePartID);
            if (!entry.PartInstanced)
                return;

            entry.ModulePartHost = entry.ModuleServicesPart.ServiceHost;
        }

        private void _FillModulePartHostInstance(Guid servicesModulePartID)
        {
            SMPEntry entry = _ModulePartsList[servicesModulePartID];
            bool firstCreation = false;
            if (!entry.HostInstanced)
            {
                firstCreation = true;
                FillModulePartHostInstance(servicesModulePartID);
            }

            if (firstCreation)
            {
                entry.ModulePartHost.StatusChange += OnServiceStatusChange;
                entry.ModulePartHost.UnhandledException += OnServiceException;
                entry.ModulePartHost.HostStatusChange += OnHostStatusChange;
            }
        }

        #endregion

        private class a
        {
            public static void OnServiceStatusChange(object sender, ServiceStatusChangeEventArgs e)
            {
            }

            public static void OnHostStatusChange(object sender, HostStatusChangeEventArgs e)
            {
            }
        }

        #region event handlers

        protected virtual void HostStatusChanged(HostStatus prevStatus)
        {
        }

        public virtual void OnServiceException(object sender, ServiceExceptionEventArgs e)
        {
            var service = sender as IServiceController;
        }

        public virtual void OnServiceStatusChange(object sender, ServiceStatusChangeEventArgs e)
        {
            var service = sender as IServiceController;
        }

        public virtual void OnHostStatusChange(object sender, HostStatusChangeEventArgs e)
        {
            var service = sender as IServiceController;
            _CalculateHostStatus();
        }

        #endregion

        #endregion

        #region internal methods

        private void _CalculateHostStatus()
        {
            HostStatus newStatus = HostStatus.Unknown;
            bool oneRunning = false;
            bool oneWarning = false;
            bool oneError = false;
            newStatus = HostStatus.RunningAll | HostStatus.WarningAll | HostStatus.ErrorAll;
            foreach (Guid id in _ModulePartsList.Keys)
            {
                SMPEntry entry = _ModulePartsList[id];
                IServiceHost srvHost = entry.ModulePartHost;

                if (srvHost == null || ((srvHost.Status & HostStatus.RunningAll) == HostStatus.Unknown))
                    newStatus = newStatus & (HostStatus) 0xFF0 | HostStatus.RunningSome;
                else
                    oneRunning = true;

                if (srvHost == null || ((srvHost.Status & HostStatus.WarningAll) == HostStatus.Unknown))
                    newStatus = newStatus & (HostStatus) 0xF0F | HostStatus.WarningSome;
                else
                    oneWarning = true;

                if (srvHost == null || ((srvHost.Status & HostStatus.ErrorAll) == HostStatus.Unknown))
                    newStatus = newStatus & (HostStatus) 0x0FF | HostStatus.ErrorSome;
                else
                    oneError = true;
            }
            if (!oneRunning) newStatus = newStatus & (HostStatus) 0xFF0 | HostStatus.Stopped;
            if (!oneWarning) newStatus = newStatus & (HostStatus) 0xF0F;
            if (!oneError) newStatus = newStatus & (HostStatus) 0x0FF;
            HostStatus prev = _Status;
            _Status = newStatus;
            HostStatusChanged(prev);
        }

        #endregion
    }
}