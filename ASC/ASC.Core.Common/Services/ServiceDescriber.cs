#region usings

using System;
using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Reflection;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Common.Services
{
    internal sealed class ServiceDescriber
    {
        public static ChannelDemand RequestChannelDemand(IServiceInfo serviceInfo)
        {
            if (serviceInfo == null)
                throw new ArgumentNullException("serviceInfo");
            return RequestChannelDemand(serviceInfo.ServiceType);
        }

        public static ChannelDemand RequestChannelDemand(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            ChannelDemand demand = null;
            var demandAttr = TypeHelper.GetFirstAttribute<ChannelDemandAttribute>(serviceType);
            demand = demandAttr != null ? demandAttr.ChannelDemand : new ChannelDemand();
            return demand;
        }

        public static int RequestServicePort(IServiceInfo serviceInfo)
        {
            if (serviceInfo == null)
                throw new ArgumentNullException("serviceInfo");
            if (serviceInfo.ModulePartInfo != null && serviceInfo.ModulePartInfo.ModuleInfo != null &&
                serviceInfo.ModulePartInfo.ModuleInfo.Type == ModuleType.CoreInfrastructure)
                return 1983;
            int port = new Random(DateTime.Now.Millisecond).Next(1000) + 20000;
            return port;
        }

        public static Type RequestServiceRealType(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            return WorkContext.ServicePublisher.ServiceLocatorRegistry.GetType(srvInfo.ID);
        }

        public static IServiceController GetServiceControllerInterface(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            var controller = service as IServiceController;
            if (controller == null)
                throw new ArgumentException(
                    String.Format("Type \"{0}\" not implements interface IServiceController"
                                  , service.GetType()
                        )
                    );
            return controller;
        }

        #region work with Service URI

        public static string GetServiceUri(IServiceInfo srvInfo, IModulePartInfo modulePartInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            if (modulePartInfo == null)
                throw new ArgumentNullException("modulePartInfo");
            if (modulePartInfo.ModuleInfo == null)
                throw new ArgumentNullException("modulePartInfo.ModuleInfo");
            return UriUtil.BuildUri("",
                                    modulePartInfo.ModuleInfo.SysName,
                                    modulePartInfo.ModuleInfo.Version.ToString(2),
                                    modulePartInfo.SysName,
                                    modulePartInfo.Version.ToString(2),
                                    srvInfo.Uri
                );
        }

        #endregion
    }
}