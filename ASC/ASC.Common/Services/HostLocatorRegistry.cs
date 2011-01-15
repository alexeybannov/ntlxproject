#region usings

using System;
using ASC.Collections;

#endregion

namespace ASC.Common.Services
{
    public class HostLocatorRegistry
        : LocatorRegistry<Guid, IServiceHost>
    {
        public void Register(Guid servicePartID, Type hostType)
        {
            if (hostType == null)
                throw new ArgumentNullException("hostType");
            ILocator<Guid, IServiceHost> locator = GetLocatorByType(typeof (HostLocator));
            if (locator == null)
            {
                locator = new HostLocator();
                RegisterServiceLocator(locator);
            }
            locator.Register(servicePartID, hostType);
        }
    }
}