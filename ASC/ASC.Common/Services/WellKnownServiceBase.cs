#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public abstract class WellKnownServiceBase
        : MarshalByRefObject, IInstancedService
    {
        private readonly Guid _ServiceInstanceID = ServiceInfoBase.DefaultInstanceID;
        private IServiceInfo _ServiceInfo;

        protected WellKnownServiceBase(IServiceInfo serviceInfo)
        {
            if (serviceInfo == null)
                throw new ArgumentNullException("serviceInfo");
            if (serviceInfo.InstancingType == ServiceInstancingType.MultipleInstances ||
                serviceInfo.InstancingType == ServiceInstancingType.MultipleFixedInstance)
                throw new InstancingTypeMismatchException(serviceInfo.Name,
                                                          "must using WellKnownServiceBase(IServiceInfo serviceInfo, Guid serviceInstanceID) constructor");
            _ServiceInfo = serviceInfo;
        }

        protected WellKnownServiceBase(IServiceInfo serviceInfo, Guid serviceInstanceID)
        {
            if (serviceInfo == null)
                throw new ArgumentNullException("serviceInfo");
            if (serviceInfo.InstancingType == ServiceInstancingType.Singleton &&
                serviceInstanceID != ServiceInfoBase.DefaultInstanceID)
                throw new InstancingTypeMismatchException(serviceInfo.Name,
                                                          "must using WellKnownServiceBase(IServiceInfo serviceInfo) constructor");
            if (serviceInfo.InstancingType != ServiceInstancingType.Singleton &&
                serviceInstanceID == ServiceInfoBase.DefaultInstanceID)
                throw new ArgumentOutOfRangeException("serviceInstanceID");
            _ServiceInfo = serviceInfo;
            _ServiceInstanceID = serviceInstanceID;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IInstancedService

        public IServiceInfo Info
        {
            get { return _ServiceInfo; }
            protected set
            {
                if (value == null) throw new ArgumentNullException();
                _ServiceInfo = value;
            }
        }

        public Guid InstanceID
        {
            get { return _ServiceInstanceID; }
        }

        #endregion
    }
}