using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using ASC.Common.Services;
using ASC.Core.Common.Services;
using ASC.Core.Hosting;
using ASC.Net;
using log4net;

namespace ASC.Core.Host
{
    public class ServicesHost : ServicesModulePartHostBase, IServicesHost
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServicesHost));

        private static readonly string servicesFolder = "Services";

        private string baseDir;


        public ServicesHost()
            : base(new ServiceInfoBase(
                typeof(IServicesHost),
                "Service container service parts module",
                null,
                "ServicesHost",
                new Version(1, 0),
                new TransportType[] { TransportType.Ipc, TransportType.Tcp },
                "ServicesHost.rem")
            ) { }

        /// <inheritdoc/>
        protected override void DoWork()
        {
            Sleep(TimeSpan.MaxValue);
        }

        protected override void BeforeStartWork()
        {
            baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var servicesPath = Path.Combine(baseDir, servicesFolder);
            if (!Directory.Exists(servicesPath)) return;

            foreach (var folder in Directory.GetDirectories(servicesPath))
            {
                try
                {
                    ((IServicesHost)this).Add(new Guid(Path.GetFileName(folder)));
                    log.InfoFormat("Discovered and added to the service Id = {0}", folder);
                }
                catch (FormatException) { }
            }
            base.BeforeStartWork();
        }

        /// <inheritdoc/>
        protected override void AfterStartWork()
        {
            ControllerStart();
        }

        /// <inheritdoc/>
        protected override void BeforeStopWork()
        {
            ControllerStop();
        }

        #region implementation FillModulePartHostInstance(Guid) in new AppDomain

        /// <inheritdoc/>
        protected override void FillModulePartHostInstance(Guid partID)
        {
            SMPEntry entry = null;
            try
            {
                log.InfoFormat("Creating and launching the service '{0}'...", partID);

                entry = this[partID];
                var serviceFolder = Path.Combine(servicesFolder, partID.ToString());
                Exception error = null;

                var domain = AppDomain.CreateDomain(partID.ToString(), null, baseDir, serviceFolder, false);
                var type = typeof(ServiceInitializer);
                var initializer = (ServiceInitializer)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, typeof(ServiceInitializer).FullName);

                var serviceRef = initializer.InitializeService(partID, Path.Combine(baseDir, serviceFolder), out error);

                if (error == null)
                {
                    entry.ModulePartHost = RemotingServices.Unmarshal(serviceRef) as IServiceHost;
                    entry.Tag = domain;
                    log.InfoFormat("Service {0} running.", partID);
                }
                else
                {
                    throw error;
                }
            }
            catch (Exception ex)
            {
                UnloadEntry(entry);
                log.ErrorFormat("Service {0} failed to start: {1}", partID, ex);
            }
        }

        #endregion

        /// <inheritdoc/>
        protected override void FillModulePartInfo(Guid servicesModulePartID)
        {
        }

        /// <inheritdoc/>
        protected override void FillModulePartInstance(Guid servicesModulePartID)
        {
        }

        /// <inheritdoc/>
        protected override void AfterStopPart(Guid servicesModulePartID)
        {
            UnloadEntry(this[servicesModulePartID]);
        }

        private void UnloadEntry(SMPEntry entry)
        {
            if (entry == null) return;
            var domain = entry.Tag as AppDomain;
            if (domain != null && domain.Id != AppDomain.CurrentDomain.Id)
            {
                AppDomain.Unload(domain);
                entry.ModulePartHost = null;
                entry.Tag = null;
                log.InfoFormat("Application domain service {0} unloaded.", entry.ServiceModulePartID);
            }
        }
    }
}