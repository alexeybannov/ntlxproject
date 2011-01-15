#region usings

using System;
using ASC.Collections;
using ASC.Reflection;

#endregion

namespace ASC.Common.Services
{
    public class ServiceLocatorRegistry
        : LocatorRegistry<Guid, IService>
    {
        public void Register(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            ServiceAttribute[] srvAttrs =
                Array.ConvertAll(
                    TypeHelper.GetAttributes(serviceType, typeof (ServiceAttribute)),
                    (o) => o as ServiceAttribute
                    );
            if (srvAttrs.Length == 0)
                throw new ServiceDefinitionException(serviceType.FullName);
            Type locatorType = typeof (Locator);
            var custLocatorAttr = TypeHelper.GetFirstAttribute<LocatorAttribute>(serviceType);
            if (custLocatorAttr != null)
                locatorType = custLocatorAttr.LocatorType;

            ILocator<Guid, IService> locator = GetLocatorByType(locatorType);
            if (locator == null)
            {
                locator = TypeInstance.Create(locatorType) as ILocator<Guid, IService>;
                RegisterServiceLocator(locator);
            }

            foreach (ServiceAttribute attr in srvAttrs)
            {
                locator.Register(attr.ServiceID, serviceType);
            }
        }

        public void Register<T>()
        {
            Register(typeof (T));
        }
    }
}