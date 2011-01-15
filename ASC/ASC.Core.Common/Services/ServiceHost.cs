#region usings

using System;
using ASC.Common.Module;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Common.Services
{
    public delegate Guid ServiceInstanceIDResolver(IServiceInfo srvInfo);

    public class ServiceHost
        : ServiceHostBase
    {
        public ServiceHost()
            : this("<unknown>")
        {
        }

        public ServiceHost(string name)
            : base(name)
        {
        }

        public ServiceHost(IModuleServicesPart servicePart)
            : this(null, servicePart)
        {
        }

        public ServiceHost(string name, IModuleServicesPart servicePart)
            : base(name, servicePart)
        {
        }

        protected override IServiceController AddService(IServiceInfo srvInfo)
        {
            WellKnownServiceHolder.WellKnownDefinition definition =
                WellKnownServiceHolder.ServiceActivationDefinition(srvInfo);
            if (definition == null)
                return base.AddService(srvInfo);
            else
                return AddWellKnownService(srvInfo);
        }

        protected override void BeforeStart()
        {
            base.BeforeStart();
        }

        protected override IServiceController CreateService(IServiceInfo srvInfo)
        {
            IServiceController srvController = null;
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                srvController = ServiceDescriber.GetServiceControllerInterface(
                    WorkContext.ServicePublisher.ServiceLocatorRegistry.CreateInstance(srvInfo.ID)
                    );
            else
                srvController = ServiceDescriber.GetServiceControllerInterface(
                    WorkContext.ServicePublisher.ServiceLocatorRegistry.CreateInstance(srvInfo.ID,
                                                                                       _ResolveServiceInstanceID(srvInfo))
                    );
            return srvController;
        }

        protected virtual WellKnownServiceHolder AddWellKnownService(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            WellKnownServiceHolder ctr = null;
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                ctr = new WellKnownServiceHolder(srvInfo);
            else
                ctr = new WellKnownServiceHolder(srvInfo, _ResolveServiceInstanceID(srvInfo));
            AddService(ctr);
            return ctr;
        }

        protected virtual Guid ResolveServiceInstanceID(IServiceInfo srvInfo)
        {
            Guid instanceID = WorkContext.ServicePublisher.ServiceInstanceIDResolver.Resolve(srvInfo);
            throw new ApplicationException(
                "impossible resolve instanceID of MultipleFixedInstances serviceinstansing type");
        }

        private Guid _ResolveServiceInstanceID(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                return ServiceInfoBase.DefaultInstanceID;
            if (srvInfo.InstancingType == ServiceInstancingType.MultipleInstances)
            {
                Guid newID = Guid.NewGuid();
                WorkContext.ServicePublisher.ServiceInstanceIDResolver.AddCorresponds(
                    srvInfo.ID,
                    newID
                    );
                return newID;
            }
            return ResolveServiceInstanceID(srvInfo);
        }
    }
}