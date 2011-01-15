#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Services
{
    public delegate Guid? ServiceInstanceIDResolver(IServiceInfo srvInfo);

    public class ServiceInstanceIDResolverHolder
    {
        private readonly IDictionary<Guid, Guid> _FixedCorrespondes = new Dictionary<Guid, Guid>();
        private readonly IList<ServiceInstanceIDResolver> _Resolvers = new List<ServiceInstanceIDResolver>(1);

        public ServiceInstanceIDResolverHolder()
        {
            _Resolvers.Add(_FixedResolver);
        }

        public void AddResolver(ServiceInstanceIDResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");
            if (!_Resolvers.Contains(resolver))
                _Resolvers.Add(resolver);
        }

        public Guid Resolve(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                return ServiceInfoBase.DefaultInstanceID;
            if (srvInfo.InstancingType == ServiceInstancingType.Singleton)
                return ServiceInfoBase.DefaultInstanceID;
            Guid? instanceID = null;
            lock (_Resolvers)
            {
                foreach (ServiceInstanceIDResolver resolver in _Resolvers)
                {
                    instanceID = resolver(srvInfo);
                    if (instanceID.HasValue)
                        break;
                }
            }
            if (!instanceID.HasValue)
                throw new ApplicationException(String.Format("couldn't resolve service \"{0}\" instance id", srvInfo));
            return instanceID.Value;
        }

        public void AddCorresponds(Guid serviceID, Guid instanceID)
        {
            lock (_FixedCorrespondes)
            {
                if (_FixedCorrespondes.ContainsKey(serviceID))
                    _FixedCorrespondes[serviceID] = instanceID;
                else
                    _FixedCorrespondes.Add(serviceID, instanceID);
            }
        }

        private Guid? _FixedResolver(IServiceInfo srvInfo)
        {
            lock (_FixedCorrespondes)
            {
                if (_FixedCorrespondes.ContainsKey(srvInfo.ID))
                    return _FixedCorrespondes[srvInfo.ID];
            }
            return null;
        }
    }
}