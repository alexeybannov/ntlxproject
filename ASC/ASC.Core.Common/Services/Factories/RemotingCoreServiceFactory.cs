#region usings

using System;
using System.Collections;
using ASC.Common.Services;
using ASC.Common.Services.Factories;
using ASC.Reflection;

#endregion

namespace ASC.Core.Common.Services.Factories
{
    public class RemotingCoreServiceFactory
        : IServiceFactory
    {
        #region IServiceFactory

        public IService GetService(Type type)
        {
            IService result = GetOrCreateService(type);
            if (result == null)
                throw new ArgumentException(
                    String.Format("type {0} not supported by this factory", type.FullName),
                    "type");
            return result;
        }

        #endregion

        private readonly Hashtable _services = Hashtable.Synchronized(new Hashtable(10));

        private IService GetOrCreateService(Type type)
        {
            var srvAttr = TypeHelper.GetFirstAttribute<ServiceAttribute>(type);
            if (srvAttr == null)
                throw new ServiceDefinitionException(type.ToString());
            var result = (IService) _services[type];
            if (result == null)
            {
                lock (_services.SyncRoot)
                {
                    result = (IService) _services[type];
                    if (result == null)
                    {
                        result = WorkContext.ServiceActivator.Activate(srvAttr.ServiceID);
                        if (!_services.ContainsKey(type))
                            _services.Add(type, result);
                    }
                }
            }
            return result;
        }
    }
}