#region usings

using System;
using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Core.Common.Services;

#endregion

namespace ASC.Core.Common.Module
{
    public abstract class ModuleServicesPart : ModuleServicesPartBase
    {
        protected ModuleServicesPart(IModulePartInfo partInfo, ServiceInfoBase[] services)
            : base(partInfo, services)
        {
        }

        protected override IServiceController CreateService(IServiceInfo srvInfo)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            return
                ServiceDescriber.GetServiceControllerInterface(
                    WorkContext.ServicePublisher.ServiceLocatorRegistry.CreateInstance(srvInfo.ID)
                    );
        }

        protected override IServiceHost CreateServiceHost()
        {
            IServiceHost srvHost = WorkContext.ServicePublisher.HostLocatorRegistry.CreateInstance(Info.ID, this);
            if (srvHost == null) srvHost = new ServiceHost(Info.SysName + " host", this);
            return srvHost;
        }
    }
}