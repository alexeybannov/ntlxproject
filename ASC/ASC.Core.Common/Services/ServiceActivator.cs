#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration;
using ASC.Net;
using ASC.Reflection;

#endregion

namespace ASC.Core.Common.Services
{
    public sealed class ServiceActivator : IServiceActivator
    {
        #region fields

        private static readonly RemotingActivator remotingActivator = new RemotingActivator();

        private readonly IDictionary<InternalServiceInfoCache.ServiceInfoEx, IService> _LocalServices =
            new Dictionary<InternalServiceInfoCache.ServiceInfoEx, IService>(5);

        private readonly object _SyncRoot = new object();

        #endregion

        #region constructors

        internal ServiceActivator()
        {
            _BuidCoreServiceInfoHash();
        }

        #endregion

        #region IServiceActivator

        public void RegisterForLocalInvoke(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            var srvInfo = new InternalServiceInfoCache.ServiceInfoEx(service.Info);
            if (service is IInstancedService)
                srvInfo.InstanceID = (service as IInstancedService).InstanceID;
            _RegisterForLocalInvoke(srvInfo, service);
        }

        public void UnRegisterForLocalInvoke(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            var srvInfo = new InternalServiceInfoCache.ServiceInfoEx(service.Info);
            if (service is IInstancedService)
                srvInfo.InstanceID = (service as IInstancedService).InstanceID;
            _UnRegisterForLocalInvoke(srvInfo, service);
        }

        public T Activate<T>()
            where T : IService
        {
            var srvAttr = TypeHelper.GetFirstAttribute<ServiceAttribute>(typeof (T));
            if (srvAttr == null)
                throw new ServiceDefinitionException(typeof (T).FullName);

            IService srv = Activate(srvAttr.ServiceID);

            if (!(srv is T))
                throw new ServiceActivationException(
                    String.Format(CommonDescriptionResource.ActivationByTypeThenByID_IncorrectType, typeof (T),
                                  srv.GetType()),
                    srvAttr.ServiceID.ToString()
                    );
            return (T) srv;
        }

        public IService Activate(Guid serviceID)
        {
            InternalServiceInfoCache.ServiceInfoEx srvInfo = _RequestServiceInfo(serviceID);

            IService srv = _InternalRemoteActivate(srvInfo);
            return srv;
        }

        public T Activate<T>(Guid serviceInstanceID)
            where T : IInstancedService
        {
            var srvAttr = TypeHelper.GetFirstAttribute<ServiceAttribute>(typeof (T));
            if (srvAttr == null)
                throw new ServiceDefinitionException(typeof (T).FullName);

            IInstancedService srv = Activate(srvAttr.ServiceID, serviceInstanceID);

            if (!(srv is T))
                throw new ServiceActivationException(
                    String.Format(CommonDescriptionResource.ActivationByTypeThenByID_IncorrectType, typeof (T),
                                  srv.GetType()),
                    srvAttr.ServiceID.ToString()
                    );
            return (T) srv;
        }

        public IInstancedService Activate(Guid serviceID, Guid serviceInstanceID)
        {
            if (serviceInstanceID == ServiceInfoBase.DefaultInstanceID)
                throw new InstancingTypeMismatchException(serviceID.ToString(),
                                                          "instanced service has default InstanceID");

            InternalServiceInfoCache.ServiceInfoEx siex = _RequestServiceInfo(serviceID, serviceInstanceID);

            IService srv = _InternalRemoteActivate(siex);
            return srv as IInstancedService;
        }

        #endregion

        #region internal activation

        private void _BuidCoreServiceInfoHash()
        {
            foreach (IServiceInfo srv in Constants.ConfigurationServices)
            {
                InternalServiceInfoCache.ServiceInfoEx siex = InternalServiceInfoCache.Get(srv.ID);
                if (siex != null) continue;
                siex = new InternalServiceInfoCache.ServiceInfoEx(srv);

                _ResolveServiceEndPoint(WorkContext.CoreAddressResolver.GetCoreHostEntry(), siex);

                lock (InternalServiceInfoCache.SynRoot)
                {
                    InternalServiceInfoCache.Add(siex);
                }
            }
        }

        private InternalServiceInfoCache.ServiceInfoEx _RequestServiceInfo(Guid serviceID)
        {
            return _RequestServiceInfo(serviceID, ServiceInfoBase.DefaultInstanceID);
        }

        private InternalServiceInfoCache.ServiceInfoEx _RequestServiceInfo(Guid serviceID, Guid serviceInstanceID)
        {
            InternalServiceInfoCache.ServiceInfoEx siex = InternalServiceInfoCache.Get(serviceID, serviceInstanceID);
            if (siex == null)
            {
                IServiceInfo srvInfo = CoreContext.ServiceLocator.GetServiceInfo(serviceID);

                siex = new InternalServiceInfoCache.ServiceInfoEx(srvInfo);
                siex.InstanceID = serviceInstanceID;

                lock (InternalServiceInfoCache.SynRoot)
                {
                    InternalServiceInfoCache.Add(siex);
                }
            }

            if (siex.ServiceEndPoint == null)
                _ResolveServiceEndPoint(siex);

            return siex;
        }

        private ConnectionHostEntry _ResolveServiceHostEntry(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            ConnectionHostEntry hostEntry = null;

            if (srvInfo.IsFixedCoreService)
            {
                hostEntry = WorkContext.CoreAddressResolver.GetCoreHostEntry();
                if (srvInfo.IsSecureService)
                    return new ConnectionHostEntry(hostEntry.HostName, CoreContext.Configuration.SecureCorePort);
                return hostEntry;
            }

            hostEntry = CoreContext.ServiceLocator.ResolveLocation(srvInfo.ID, srvInfo.InstanceID);

            if (hostEntry == null)
                throw new ServiceActivationException(
                    String.Format(
                        CommonDescriptionResource.ServiceActivationException_Message_LocationNotFound,
                        srvInfo
                        )
                    );
            return hostEntry;
        }

        private ServiceEndPoint _ResolveServiceEndPoint(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            return _ResolveServiceEndPoint(_ResolveServiceHostEntry(srvInfo), srvInfo);
        }

        private ServiceEndPoint _ResolveServiceEndPoint(ConnectionHostEntry hostEntry,
                                                        InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            srvInfo.ServiceEndPoint = new ServiceEndPoint(
                new ConnectionHostEntry(hostEntry.HostName, hostEntry.Port),
                TransportSelector.Select(hostEntry, srvInfo.TransportTypes),
                srvInfo.Uri
                );
            return srvInfo.ServiceEndPoint;
        }

        private IService _InternalRemoteActivate(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            IService srv = null;
            lock (_SyncRoot)
            {
                if (_LocalServices.ContainsKey(srvInfo))
                {
                    srv = _LocalServices[srvInfo];
                    LogHolder.Log("ASC.Core.Common")
                        .DebugFormat("service {0} is local activated", srv.Info);
                }
            }
            if (srv == null)
            {
                srv = remotingActivator.Activate(srvInfo);
                _CheckActivatedService(srvInfo, srv);
            }
            return srv;
        }

        private void _CheckActivatedService(InternalServiceInfoCache.ServiceInfoEx srvInfoEx, IService service)
        {
            if (srvInfoEx == null)
                throw new ArgumentNullException("srvInfoEx");
            Exception except = null;
            if (service == null)
            {
                except = new ServiceActivationException(srvInfoEx.Name);
            }
            else
            {
                IServiceInfo srvInfo = null;
                try
                {
                    srvInfo = service.Info;

                    if (srvInfo.ID != srvInfoEx.ID)
                    {
                        except = new ServiceActivationException(
                            String.Format(
                                CommonDescriptionResource.ServiceActivationException_Reason_NotThisServiceType,
                                srvInfoEx.ServiceEndPoint.ServiceUri),
                            srvInfoEx.Name
                            );
                    }
                    else
                    {
                        if (srvInfoEx.InstancingType != ServiceInstancingType.Singleton)
                        {
                            var instancedSrv = (service as IInstancedService);
                            if (instancedSrv == null)
                            {
                                except = new ServiceActivationException(srvInfoEx.Name);
                            }
                            else
                            {
                                if (instancedSrv.InstanceID != srvInfoEx.InstanceID)
                                {
                                    except = new ServiceActivationException(
                                        String.Format(
                                            CommonDescriptionResource.
                                                ServiceActivationException_Reason_NotThisServiceInstance,
                                            srvInfoEx.ServiceEndPoint.ServiceUri),
                                        srvInfoEx.Name
                                        );
                                }
                            }
                        }
                    }
                }

                catch (Exception exc)
                {
                    except = new ServiceActivationException(
                        String.Format(CommonDescriptionResource.ServiceActivationException_Reason_NotAccessable,
                                      srvInfoEx.ServiceEndPoint.ConnectionHostEntry),
                        srvInfoEx.Name,
                        exc
                        );
                }
            }
            if (except != null)
            {
                InternalServiceInfoCache.Remove(srvInfoEx.Key);
                throw except;
            }
        }

        private void _RegisterForLocalInvoke(InternalServiceInfoCache.ServiceInfoEx srvInfoEx, IService service)
        {
            lock (_SyncRoot)
            {
                if (_LocalServices.ContainsKey(srvInfoEx))
                    _LocalServices[srvInfoEx] = service;
                else
                    _LocalServices.Add(srvInfoEx, service);
            }
        }

        private void _UnRegisterForLocalInvoke(InternalServiceInfoCache.ServiceInfoEx srvInfoEx, IService service)
        {
            lock (_SyncRoot)
            {
                if (_LocalServices.ContainsKey(srvInfoEx))
                    _LocalServices.Remove(srvInfoEx);
            }
        }

        #endregion
    }
}