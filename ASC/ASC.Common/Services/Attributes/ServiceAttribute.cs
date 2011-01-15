#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ServiceAttribute : Attribute
    {
        private readonly ServiceInstancingType _InstancingType;
        private readonly Guid _ServiceID;

        public ServiceAttribute(string serviceID)
            : this(serviceID, ServiceInstancingType.Singleton)
        {
        }

        public ServiceAttribute(string serviceID, ServiceInstancingType instancingType)
        {
            if (null == serviceID)
                throw new ArgumentNullException("serviceID");
            _ServiceID = new Guid(serviceID);
            _InstancingType = instancingType;
        }

        public ServiceAttribute(Guid serviceID)
        {
            if (Guid.Empty == serviceID)
                throw new ArgumentNullException("serviceID");
            _ServiceID = serviceID;
        }

        public Guid ServiceID
        {
            get { return _ServiceID; }
        }

        public ServiceInstancingType InstancingType
        {
            get { return _InstancingType; }
        }
    }
}