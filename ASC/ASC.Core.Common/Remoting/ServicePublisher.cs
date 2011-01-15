#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ASC.Common.Services;
using ASC.Core.Common.Services;
using ASC.Net;
using ASC.Reflection;
using ASC.Runtime.Remoting.Channels;
using log4net;

#endregion

namespace ASC.Core.Common.Remoting
{
    public class ServicePublisher : IServicePublisher
    {
        private readonly ServiceLocatorRegistry _ServiceLocatorRegistry = new ServiceLocatorRegistry();
        private readonly HostLocatorRegistry _HostLocatorRegistry = new HostLocatorRegistry();

        private readonly ServiceInstanceIDResolverHolder _ServiceInstanceIDResolver =
            new ServiceInstanceIDResolverHolder();

        private static readonly ILog log = LogManager.GetLogger(typeof (ServicePublisher));

        internal ServicePublisher()
        {
        }

        #region IServicePublisher

        public ServiceLocatorRegistry ServiceLocatorRegistry
        {
            get { return _ServiceLocatorRegistry; }
        }

        public HostLocatorRegistry HostLocatorRegistry
        {
            get { return _HostLocatorRegistry; }
        }

        public ServiceInstanceIDResolverHolder ServiceInstanceIDResolver
        {
            get { return _ServiceInstanceIDResolver; }
        }

        public IService Publish(IServiceInfo srvInfo)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            if (srvInfo.ServiceType == null) throw new ArgumentNullException("srvInfo.ServiceType");
            log.DebugFormat("publishing service \"{0}\"...", srvInfo.Name);
            if (!TypeHelper.ImplementInterface(srvInfo.ServiceType, typeof (IService)))
                throw new ServiceDefinitionException(srvInfo.ServiceType.FullName);
            if (srvInfo.InstancingType != ServiceInstancingType.Singleton)
                throw new InstancingTypeMismatchException(srvInfo.Name,
                                                          "must use other method for publishing instanced service Publish(IServiceInfo srvInfo, Guid serviceInstanceID)");
            return _Publish_NotCheckInstancingType(srvInfo, ServiceInfoBase.DefaultInstanceID);
        }

        public IInstancedService Publish(IServiceInfo srvInfo, Guid serviceInstanceID)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            if (srvInfo.ServiceType == null) throw new ArgumentNullException("srvInfo.ServiceType");
            log.DebugFormat("publishing service \"{0}\"[instance={1}]...", srvInfo.Name, serviceInstanceID);
            if (!TypeHelper.ImplementInterface(srvInfo.ServiceType, typeof (IService)))
                throw new ServiceDefinitionException(srvInfo.ServiceType.FullName);
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                throw new InstancingTypeMismatchException(srvInfo.Name,
                                                          "must use other method for publishing singleton service Publish(IServiceInfo srvInfo)");
            if (!TypeHelper.ImplementInterface(srvInfo.ServiceType, typeof (IInstancedService)))
                throw new InstancingTypeMismatchException(srvInfo.Name,
                                                          "instanced service Type must implement IInstancedService");
            if (serviceInstanceID == ServiceInfoBase.DefaultInstanceID)
                throw new InstancingTypeMismatchException(srvInfo.Name, "instanced service has default InstanceID");

            IService srv = _Publish_NotCheckInstancingType(srvInfo, serviceInstanceID);
            return srv as IInstancedService;
        }

        public void Publish(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            log.DebugFormat("publishing service \"{0}\"...", service.Info.Name);
            if (!TypeHelper.IsSameOrParent(typeof (MarshalByRefObject), service.GetType()))
                throw new ServicePublishingException(service.Info.Name,
                                                     CommonDescriptionResource.
                                                         ServicePublishingException_NotMarshalByRef);
            Guid instanceID = ServiceInfoBase.DefaultInstanceID;
            if (service is IInstancedService && service.Info.InstancingType != ServiceInstancingType.Singleton)
            {
                instanceID = (service as IInstancedService).InstanceID;
                if (instanceID == ServiceInfoBase.DefaultInstanceID)
                    throw new InstancingTypeMismatchException(service.Info.Name,
                                                              "service is instanced but InstanceID has default value");
            }
            InternalServiceInfoCache.ServiceInfoEx siex = _GetInfoEx(service.Info, instanceID);
            try
            {
                _PublishInternal(service, siex);
            }
            catch (ServicePublishingException)
            {
                throw;
            }
            catch (Exception exc)
            {
                throw new ServicePublishingException(service.Info.Name, exc);
            }
        }

        public void UnPublish(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            log.DebugFormat("unpublishing service \"{0}\"...", service.Info.Name);
            if (!TypeHelper.IsSameOrParent(typeof (MarshalByRefObject), service.GetType()))
                throw new ArgumentException(CommonDescriptionResource.SevicePublisher_Publish_IncorrectType, "service");
            Guid instanceID = ServiceInfoBase.DefaultInstanceID;
            if (service is IInstancedService && service.Info.InstancingType != ServiceInstancingType.Singleton)
            {
                instanceID = (service as IInstancedService).InstanceID;
                if (instanceID == ServiceInfoBase.DefaultInstanceID)
                    throw new InstancingTypeMismatchException(service.Info.Name,
                                                              "service is instanced but InstanceID has default value");
            }
            InternalServiceInfoCache.ServiceInfoEx siex = _GetInfoEx(service.Info, instanceID);
            _UnPublishInternal(service, siex);
        }

        public void UnPublish(IServiceInfo srvInfo)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            log.DebugFormat("unpublishing service \"{0}\"...", srvInfo.Name);
            if (srvInfo.InstancingType != ServiceInstancingType.Singleton)
                throw new InstancingTypeMismatchException(srvInfo.ToString(),
                                                          "must use other method for instanced services UnPublish(IServiceInfo srvInfo, Guid serviceInstanceID)");
            InternalServiceInfoCache.ServiceInfoEx siex = _GetInfoEx(srvInfo);
            if (!siex.IsWellKnownService)
            {
                throw new NotImplementedException("Use UnPublish(IService service)");
            }
            else
            {
                _UnRegisterWellKnownService(siex);
            }
        }

        public void UnPublish(IServiceInfo srvInfo, Guid serviceInstanceID)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            log.DebugFormat("unpublishing service \"{0}\"[{1}]...", srvInfo.Name, serviceInstanceID);
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                throw new InstancingTypeMismatchException(srvInfo.ToString(),
                                                          "must use other method for singleton services UnPublish(IServiceInfo srvInfo)");
            InternalServiceInfoCache.ServiceInfoEx siex = _GetInfoEx(srvInfo, serviceInstanceID);
            if (!siex.IsWellKnownService)
            {
                throw new NotImplementedException("Use UnPublish(IService service)");
            }
            else
            {
                _UnRegisterWellKnownService(siex);
            }
        }

        #endregion

        private IService _Publish_NotCheckInstancingType(IServiceInfo srvInfo, Guid serviceInstanceID)
        {
            InternalServiceInfoCache.ServiceInfoEx siex = _GetInfoEx(srvInfo);
            siex.InstanceID = serviceInstanceID;
            if (siex.IsWellKnownService)
            {
                if (siex.WKD.ServerActivated)
                    _RegisterWellKnownServerService(siex);
                else
                {
                    throw new NotSupportedException("client activation not supported yet");
                }
                return null;
            }
            else
            {
                IService service = null;
                if (siex.InstancingType == ServiceInstancingType.Singleton)
                    service = ServiceLocatorRegistry.CreateInstance(srvInfo.ID);
                else
                    service = ServiceLocatorRegistry.CreateInstance(srvInfo.ID, siex.InstanceID);
                if (service == null)
                {
                    if (siex.InstancingType == ServiceInstancingType.Singleton)
                        service = TypeInstance.Create(srvInfo.ServiceType) as IService;
                    else
                        service = TypeInstance.Create(srvInfo.ServiceType, siex.InstanceID) as IService;
                }
                if (service == null)
                    throw new ServicePublishingException(srvInfo.Name);
                _PublishInternal(service, siex);
                return service;
            }
        }

        private ObjRef _PublishInternal(IService service, InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (service == null || srvInfo == null || srvInfo.ServiceType == null)
                throw new ArgumentNullException("service");
            _EnsureFreeServiceUri(srvInfo, service.GetType());

            _StartupChannels(srvInfo);
            log.DebugFormat("marshal service \"{0}\"[{2}] at uri \"{1}\"...", service.Info.Name, service.Info.Uri,
                            service.GetType());
            ObjRef objRef = RemotingServices.Marshal(service as MarshalByRefObject, srvInfo.Uri, null);
            _NotifyAboutPublishing(srvInfo);
            WorkContext.ServiceActivator.RegisterForLocalInvoke(service);
            return objRef;
        }

        private void _UnPublishInternal(IService service, InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (service == null || service.Info == null || service.Info.ServiceType == null)
                throw new ArgumentNullException("service");
            var _service = service as MarshalByRefObject;
            if (!RemotingServices.Disconnect(_service))
            {
                log.WarnFormat("RemotingServices.Disconnect({0}) return FALSE",
                               _service);
            }
            WorkContext.RemotingSubsystem.DeattachServiceFromChannels(srvInfo.Identity, true);

            _NotifyAboutUnPublishing(srvInfo);
            WorkContext.ServiceActivator.UnRegisterForLocalInvoke(service);
        }

        private void _StartupChannels(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (srvInfo == null || srvInfo.ServiceType == null)
                throw new ArgumentNullException();

            ChannelDemand channelDemand = ServiceDescriber.RequestChannelDemand(srvInfo);

            int servicePort = _RequestServicePort(srvInfo);

            foreach (TransportType type in srvInfo.TransportTypes)
            {
                IChannel channel = WorkContext.RemotingSubsystem.StartupServerChannel(
                    type,
                    servicePort,
                    channelDemand,
                    CoreContext.Configuration.RemotingSubsystemConfiguration.GetFirstServerSinkProvider()
                    );

                if (channel is IChannelWithServices)
                    (channel as IChannelWithServices).AttachService(srvInfo.Identity);
            }
        }

        private void _RegisterWellKnownServerService(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (!srvInfo.IsWellKnownService)
                throw new ArgumentNullException("srvInfo.WKD");
            if (!srvInfo.WKD.ServerActivated)
                throw new ArgumentException("srvInfo.WKD");
            WellKnownObjectMode objectMode = srvInfo.WKD.ObjectMode;

            Type serviceType = srvInfo.WKD.RealServiceType;
            if (serviceType == null)
            {
                serviceType = srvInfo.ServiceType;
            }

            if (!TypeHelper.IsSameOrParent(typeof (MarshalByRefObject), serviceType))
                throw new ServicePublishingException(srvInfo.Name,
                                                     CommonDescriptionResource.
                                                         ServicePublishingException_NotMarshalByRef);

            _EnsureFreeServiceUri(srvInfo, serviceType);

            _StartupChannels(srvInfo);

            var wkste =
                new WellKnownServiceTypeEntry(
                    serviceType,
                    srvInfo.Uri,
                    objectMode
                    );
            log.InfoFormat("register well known server activated service \"{0}\"[{2}] at uri \"{1}\"...", srvInfo.Name,
                           srvInfo.Uri, serviceType);
            RemotingConfiguration.RegisterWellKnownServiceType(
                wkste
                );

            _NotifyAboutPublishing(srvInfo);
        }

        private void _RegisterWellKnownClientService(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (!srvInfo.IsWellKnownService)
                throw new ArgumentNullException("srvInfo.WKD");
            if (srvInfo.WKD.ServerActivated)
                throw new ArgumentException("srvInfo.WKD");

            Type serviceType = srvInfo.WKD.RealServiceType;
            if (serviceType == null)
            {
                serviceType = srvInfo.ServiceType;
            }

            if (!TypeHelper.IsSameOrParent(typeof (MarshalByRefObject), serviceType))
                throw new ServicePublishingException(srvInfo.Name,
                                                     CommonDescriptionResource.
                                                         ServicePublishingException_NotMarshalByRef);

            _EnsureFreeServiceUri(srvInfo, serviceType);

            _StartupChannels(srvInfo);

            var acte =
                new ActivatedServiceTypeEntry(
                    srvInfo.ServiceType
                    );
            log.InfoFormat("register well known client activated service \"{0}\" at uri \"{1}\"...", srvInfo.Name,
                           srvInfo.Uri);
            RemotingConfiguration.RegisterActivatedServiceType(
                acte
                );

            _NotifyAboutPublishing(srvInfo);
        }

        private void _UnRegisterWellKnownService(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (srvInfo.WKD.ServerActivated)
                _UnRegisterServerWellKnownService(srvInfo);
            else
                _UnRegisterClientWellKnownService(srvInfo);

            _NotifyAboutUnPublishing(srvInfo);
        }

        private void _UnRegisterServerWellKnownService(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            WellKnownServiceTypeEntry entry = null;
            foreach (WellKnownServiceTypeEntry _entry in RemotingConfiguration.GetRegisteredWellKnownServiceTypes())
            {
                if (
                    _entry.ObjectUri == srvInfo.Uri
                    &&
                    _entry.ObjectType == srvInfo.WKD.RealServiceType
                    )
                {
                    entry = _entry;
                    break;
                }
            }
            if (entry == null) return;
            WorkContext.RemotingSubsystem.DeattachServiceFromChannels(srvInfo.InstanceID, true);
        }

        private void _UnRegisterClientWellKnownService(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            ActivatedServiceTypeEntry entry = null;
            foreach (ActivatedServiceTypeEntry _entry in RemotingConfiguration.GetRegisteredActivatedServiceTypes())
            {
                if (
                    _entry.ObjectType == srvInfo.WKD.RealServiceType
                    )
                {
                    entry = _entry;
                    break;
                }
            }
            if (entry == null) return;
            WorkContext.RemotingSubsystem.DeattachServiceFromChannels(srvInfo.Identity, true);
        }

        private void _EnsureFreeServiceUri(InternalServiceInfoCache.ServiceInfoEx srvInfo, Type realServiceType)
        {
            Type serviceType = RemotingServices.GetServerTypeForUri(srvInfo.Uri);
            if (serviceType != null)
            {
                if (serviceType == realServiceType)

                    throw new ServicePublishingException(
                        srvInfo.Name,
                        String.Format(CommonDescriptionResource.ServicePublishingException_Allready, serviceType)
                        );
                else

                    throw new ServicePublishingException(
                        srvInfo.Name,
                        String.Format(CommonDescriptionResource.ServicePublishingException_Uri_Exists, srvInfo.Uri,
                                      serviceType)
                        );
            }
        }

        private int _RequestServicePort(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            int portNumber = 0;

            if (srvInfo.IsFixedCoreService)
            {
                portNumber = srvInfo.IsSecureService
                                 ? CoreContext.Configuration.SecureCorePort
                                 : WorkContext.CoreAddressResolver.GetCoreHostEntry().Port;
            }
            else
            {
                if (srvInfo.HostingModel == ServiceHostingModel.Flexible)
                {
                    portNumber = TcpPortResolver.GetFreePort();
                }
                else
                {
                    ConnectionHostEntry hostEntry = CoreContext.ServiceLocator.ResolveLocation(srvInfo.ID,
                                                                                               srvInfo.InstanceID);
                    if (hostEntry != null) portNumber = hostEntry.Port;
                }
            }
            srvInfo.ServiceEndPoint = new ServiceEndPoint(
                ConnectionHostEntry.GetLocalhost(portNumber),
                TransportSelector.Select(ConnectionHostEntry.GetLocalhost(portNumber), srvInfo.TransportTypes),
                srvInfo.Uri
                );

            return portNumber;
        }

        private InternalServiceInfoCache.ServiceInfoEx _GetInfoEx(IServiceInfo srvInfo)
        {
            return _GetInfoEx(srvInfo, ServiceInfoBase.DefaultInstanceID);
        }

        private InternalServiceInfoCache.ServiceInfoEx _GetInfoEx(IServiceInfo srvInfo, Guid instanceID)
        {
            var key = new InternalServiceInfoCache.ServiceInfoEx.SIKey(srvInfo.ID, instanceID);
            InternalServiceInfoCache.ServiceInfoEx siex = null;
            lock (InternalServiceInfoCache.SynRoot)
            {
                if (!InternalServiceInfoCache.Contains(srvInfo.ID, instanceID))
                {
                    siex = new InternalServiceInfoCache.ServiceInfoEx(srvInfo);
                    siex.InstanceID = instanceID;
                    siex.HostingModel = siex.IsFixedCoreService
                                            ? ServiceHostingModel.DirectAddress
                                            : CoreContext.ServiceLocator.GetServiceHostingModel(srvInfo.ID);
                    siex.WKD = WellKnownServiceHolder.ServiceActivationDefinition(srvInfo);
                    InternalServiceInfoCache.Add(siex);
                }
                else
                    siex = InternalServiceInfoCache.Get(key);
            }
            return siex;
        }

        private void _NotifyAboutPublishing(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            log.DebugFormat("ServicePublisher successfully published \"{0}\"[{1}] as {2}",
                            srvInfo.Name,
                            srvInfo.InstanceID,
                            srvInfo.ServiceEndPoint.ConnectionHostEntry);
            try
            {
                if (!srvInfo.IsFixedCoreService)
                {
                    CoreContext.ServiceLocator.ServiceStartedUp(srvInfo, srvInfo.InstanceID,
                                                                srvInfo.ServiceEndPoint.ConnectionHostEntry);
                }
            }
            catch (ServiceInstanceNotFoundException)
            {
                log.WarnFormat("ServiceLocator.ServiceStartedUp:: instance not found exception");
            }
        }

        private void _NotifyAboutUnPublishing(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            log.DebugFormat("ServicePublisher successfully UNpublished \"{0}\"[{1}]",
                            srvInfo.Name,
                            srvInfo.InstanceID);
            try
            {
                if (!srvInfo.IsFixedCoreService)
                {
                    CoreContext.ServiceLocator.ServiceShutDowned(srvInfo.ID, srvInfo.InstanceID);
                }
            }
            catch (ServiceInstanceNotFoundException)
            {
                log.WarnFormat("ServiceLocator.ServiceShutDowned:: instance not found exception");
            }
        }
    }
}