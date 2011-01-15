#region usings

using System;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Reflection;

#endregion

namespace ASC.Common.Module
{
    [Serializable]
    public abstract class ModuleServicesPartBase : ModulePartBase, IModuleServicesPart
    {
        private readonly ServiceInfoBase[] _Services;
        private IServiceHost _Host;

        protected ModuleServicesPartBase(IModulePartInfo partInfo, ServiceInfoBase[] services)
            :
                base(partInfo)
        {
            _Services = ArrayUtil.CopyClonable(services);
            if (_Services != null)
                for (int i = 0; i < _Services.Length; i++)
                    _Services[i].ModulePartInfo = Info;
        }

        #region IModuleServicesPart Members

        public IServiceInfo[] Services
        {
            get { return (IServiceInfo[]) _Services.Clone(); }
        }

        public IServiceHost ServiceHost
        {
            get
            {
                if (_Host == null)
                    _Host = CreateServiceHost();
                return _Host;
            }
        }

        public IServiceController CreateServiceInstance(IServiceInfo srvInfo)
        {
            if (srvInfo == null || srvInfo.ServiceType == null) throw new ArgumentNullException("srvInfo");
            bool finded = false;
            foreach (IServiceInfo srv in _Services)
                if (srv.ID == srvInfo.ID && srv.ServiceType == srvInfo.ServiceType)
                {
                    finded = true;
                    break;
                }
            if (!finded) throw new ServiceNotFoundException(srvInfo.Name);
            IServiceController srvController = CreateService(srvInfo);
            if (srvController == null)
            {
                object puresrv = TypeInstance.Create(srvInfo.ServiceType);
                if (
                    !TypeHelper.ImplementInterface(puresrv.GetType(), typeof (IService))
                    ||
                    !TypeHelper.ImplementInterface(puresrv.GetType(), typeof (IServiceController))
                    )
                    throw new ServiceDefinitionException(srvInfo.Name);
                srvController = puresrv as IServiceController;
            }
            return srvController;
        }

        #endregion

        protected virtual IServiceHost CreateServiceHost()
        {
            return new ServiceHostBase(this);
        }

        protected virtual IServiceController CreateService(IServiceInfo srvInfo)
        {
            return null;
        }
    }
}