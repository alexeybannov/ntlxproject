#region usings

using System;
using System.Collections.Generic;
using System.Threading;
using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Core.Configuration;
using ASC.Net;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Common.Services
{
    public sealed class InternalServiceInfoCache
    {
        private static readonly IDictionary<ServiceInfoEx.SIKey, ServiceInfoEx> _SrvInfo =
            new Dictionary<ServiceInfoEx.SIKey, ServiceInfoEx>(10);

        private static readonly ReaderWriterLock _RWLock = new ReaderWriterLock();
        private static readonly TimeSpan _LockTryAcqurePeriod = new TimeSpan(0, 0, 5);

        internal static object SynRoot = new object();
        private static bool _CoreCacheBuilded;

        #region public methods

        #region get this

        public static ServiceInfoEx Get(Guid serviceID)
        {
            _BuildCoreCache();
            return Get(new ServiceInfoEx.SIKey(serviceID, ServiceInfoBase.DefaultInstanceID));
        }

        public static ServiceInfoEx Get(Guid serviceID, Guid serviceInstanceID)
        {
            _BuildCoreCache();
            return Get(new ServiceInfoEx.SIKey(serviceID, serviceInstanceID));
        }

        public static ServiceInfoEx Get(ServiceInfoEx.SIKey key)
        {
            _BuildCoreCache();
            ServiceInfoEx siex = null;
            try
            {
                _RWLock.AcquireReaderLock(_LockTryAcqurePeriod);
                if (_SrvInfo.ContainsKey(key))
                    siex = _SrvInfo[key];
            }
            catch (ApplicationException)
            {
                throw;
            }
            finally
            {
                _RWLock.ReleaseReaderLock();
            }
            return siex;
        }

        #endregion

        public static bool Contains(Guid serviceID)
        {
            _BuildCoreCache();
            return Contains(new ServiceInfoEx.SIKey(serviceID, ServiceInfoBase.DefaultInstanceID));
        }

        public static bool Contains(Guid serviceID, Guid serviceInstanceID)
        {
            _BuildCoreCache();
            return Contains(new ServiceInfoEx.SIKey(serviceID, serviceInstanceID));
        }

        public static bool Contains(ServiceInfoEx.SIKey key)
        {
            _BuildCoreCache();
            try
            {
                _RWLock.AcquireReaderLock(_LockTryAcqurePeriod);
                return _SrvInfo.ContainsKey(key);
            }
            catch (ApplicationException)
            {
                throw;
            }
            finally
            {
                _RWLock.ReleaseReaderLock();
            }
        }

        #region data entry

        public static void Add(ServiceInfoEx srvInfo)
        {
            _BuildCoreCache();
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            try
            {
                _RWLock.AcquireWriterLock(_LockTryAcqurePeriod);
                if (Contains(srvInfo.Key))
                    throw new ArgumentException(
                        String.Format("Inforamtion abount \"{0}\"(instance={1}) allready exists in cache", srvInfo.Name,
                                      srvInfo.InstanceID));
                _SrvInfo.Add(srvInfo.Key, srvInfo);
            }
            catch (ApplicationException)
            {
                throw;
            }
            finally
            {
                _RWLock.ReleaseWriterLock();
            }
        }

        public static void Remove(ServiceInfoEx.SIKey key)
        {
            try
            {
                _RWLock.AcquireWriterLock(_LockTryAcqurePeriod);
                if (_SrvInfo.ContainsKey(key))
                    _SrvInfo.Remove(key);
            }
            catch (ApplicationException)
            {
                throw;
            }
            finally
            {
                _RWLock.ReleaseWriterLock();
            }
        }

        #endregion

        #endregion

        private static void _BuildCoreCache()
        {
            try
            {
                _RWLock.AcquireWriterLock(_LockTryAcqurePeriod);
                if (_CoreCacheBuilded) return;
                foreach (ServiceInfoBase srvInfo in Constants.ConfigurationServices)
                {
                    var siEx = new ServiceInfoEx(srvInfo)
                                   {
                                       HostingModel = ServiceHostingModel.DirectAddress,
                                       WKD = WellKnownServiceHolder.ServiceActivationDefinition(srvInfo)
                                   };
                    _SrvInfo.Add(siEx.Key, siEx);
                }
                foreach (ServiceInfoBase srvInfo in Core.Users.Constants.UsersServices)
                {
                    var siEx = new ServiceInfoEx(srvInfo)
                                   {
                                       HostingModel = ServiceHostingModel.DirectAddress,
                                       WKD = WellKnownServiceHolder.ServiceActivationDefinition(srvInfo)
                                   };
                    _SrvInfo.Add(siEx.Key, siEx);
                }
                _CoreCacheBuilded = true;
            }
            finally
            {
                _RWLock.ReleaseWriterLock();
            }
        }

        [Serializable]
        public class ServiceInfoEx
            : IServiceInfo
        {
            private IServiceInfo _SrvInfo;
            private readonly SIKey _Key;
            private readonly bool _IsFixedCoreService;

            public ServiceInfoEx(IServiceInfo srvInfo)
            {
                if (srvInfo == null) throw new ArgumentNullException();
                _SrvInfo = srvInfo;
                _Key = new SIKey(srvInfo.ID, ServiceInfoBase.DefaultInstanceID);
                if (Hosting.Constants.CoreHostServiceInfo.ServiceType == srvInfo.ServiceType)
                    _IsFixedCoreService = true;
                else
                {
                    foreach (ServiceInfoBase coreServInfo in Constants.ConfigurationServices)
                    {
                        if (srvInfo.ServiceType == coreServInfo.ServiceType)
                        {
                            _IsFixedCoreService = true;
                            break;
                        }
                    }
                    if (!_IsFixedCoreService)
                        foreach (ServiceInfoBase coreServInfo in Core.Users.Constants.UsersServices)
                        {
                            if (srvInfo.ServiceType == coreServInfo.ServiceType)
                            {
                                _IsFixedCoreService = true;
                                break;
                            }
                        }
                }
            }

            #region IServiceInfo

            public string ServiceTypeName
            {
                get { return _SrvInfo.ServiceTypeName; }
            }

            public Type ServiceType
            {
                get { return _SrvInfo.ServiceType; }
            }

            public IModulePartInfo ModulePartInfo
            {
                get { return _SrvInfo.ModulePartInfo; }
            }

            public ServiceInstancingType InstancingType
            {
                get { return _SrvInfo.InstancingType; }
            }

            #endregion

            #region IFixedIdentification

            public Guid ID
            {
                get { return _SrvInfo.ID; }
            }

            public string Name
            {
                get { return _SrvInfo.Name; }
            }

            public string Description
            {
                get { return _SrvInfo.Description; }
            }

            #endregion

            #region ISysObject

            public string SysName
            {
                get { return _SrvInfo.SysName; }
            }

            public Version Version
            {
                get { return _SrvInfo.Version; }
            }

            #endregion

            #region IDiscoverable

            public TransportType[] TransportTypes
            {
                get { return _SrvInfo.TransportTypes; }
            }

            public string Uri
            {
                get { return _SrvInfo.Uri; }
            }

            #endregion

            public ServiceHostingModel HostingModel { get; set; }

            public Guid InstanceID
            {
                get { return _Key.ServiceInstanceID; }
                set { _Key.ServiceInstanceID = value; }
            }

            public WellKnownServiceHolder.WellKnownDefinition WKD { get; set; }

            public bool IsWellKnownService
            {
                get { return WKD != null; }
            }

            public SIKey Key
            {
                get { return _Key; }
            }

            public Guid Identity
            {
                get { return Key.Indentity; }
            }

            [Serializable]
            public class SIKey
            {
                public SIKey(Guid serviceID, Guid instanceID)
                {
                    ServiceID = serviceID;
                    ServiceInstanceID = instanceID;
                }

                public Guid ServiceID;

                public Guid ServiceInstanceID;

                public Guid Indentity
                {
                    get
                    {
                        if (ServiceInstanceID == ServiceInfoBase.DefaultInstanceID)
                            return ServiceID;
                        return ServiceInstanceID;
                    }
                }

                public override int GetHashCode()
                {
                    return Indentity.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    var key = obj as SIKey;
                    if (key == null) return false;
                    return Indentity.Equals(key.Indentity);
                }
            }

            public ServiceEndPoint ServiceEndPoint { get; set; }

            public bool IsFixedCoreService
            {
                get { return _IsFixedCoreService; }
            }

            public ChannelDemand Demand
            {
                get { return ServiceDescriber.RequestChannelDemand(this); }
            }

            public bool IsSecureService
            {
                get { return Demand != null ? Demand.Secure : false; }
            }

            public IServiceInfo ServiceInfo
            {
                get { return _SrvInfo; }
            }

            public override string ToString()
            {
                return _SrvInfo.ToString();
            }

            public override bool Equals(object obj)
            {
                var ex = obj as ServiceInfoEx;
                if (ex == null) base.Equals(obj);
                return Key.Equals(ex.Key);
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }
        }
    }
}