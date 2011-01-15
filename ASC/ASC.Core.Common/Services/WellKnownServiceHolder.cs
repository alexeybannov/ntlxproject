#region usings

using System;
using System.Runtime.Remoting;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Common.Services
{
    public class WellKnownServiceHolder : ServiceControllerBase
    {
        private readonly WellKnownDefinition _wellKnownDefinition;

        internal WellKnownServiceHolder(IServiceInfo srvInfo) :
            base(srvInfo)
        {
            _wellKnownDefinition = ServiceActivationDefinition(srvInfo);
            if (_wellKnownDefinition == null) throw new ServiceDefinitionException(srvInfo.Name);
        }

        internal WellKnownServiceHolder(IServiceInfo srvInfo, Guid instanceID) :
            base(srvInfo, instanceID)
        {
            _wellKnownDefinition = ServiceActivationDefinition(srvInfo);
            if (_wellKnownDefinition == null) throw new ServiceDefinitionException(srvInfo.Name);
        }

        protected override void StartInternal()
        {
            if (Info.InstancingType == ServiceInstancingType.Singleton)
                WorkContext.ServicePublisher.Publish(Info);
            else
                WorkContext.ServicePublisher.Publish(Info, InstanceID);
        }

        protected override void StopInternal()
        {
            if (Info.InstancingType == ServiceInstancingType.Singleton)
                WorkContext.ServicePublisher.UnPublish(Info);
            else
                WorkContext.ServicePublisher.UnPublish(Info, InstanceID);
        }

        [Serializable]
        public class WellKnownDefinition
        {
            public WellKnownObjectMode ObjectMode;
            public Type RealServiceType;
            public bool ServerActivated;

            public WellKnownDefinition(WellKnownObjectMode objectMode, bool serverActivated, Type realServiceType)
            {
                ObjectMode = objectMode;
                ServerActivated = serverActivated;
                RealServiceType = realServiceType;
            }
        }

        internal static WellKnownDefinition ServiceActivationDefinition(IServiceInfo srvInfo)
        {
            if (srvInfo == null) throw new ArgumentNullException("srvInfo");
            Type realServiceType = WorkContext.ServicePublisher.ServiceLocatorRegistry.GetType(srvInfo.ID);
            if (realServiceType == null) realServiceType = srvInfo.ServiceType;
            WellKnownDefinition wkd = null;
            return wkd;
        }
    }
}