#region usings

using System;
using ASC.Common.Services;

#endregion

namespace ASC.Common.Module
{
    [Serializable]
    public class ModuleServicesPartInfo : ModulePartInfoBase, IModuleServicesPartInfo
    {
        private readonly ServiceInfoBase[] services;

        public ModuleServicesPartInfo(Guid id, string name, string description, string sysName,
                                      ModulePartType modulePartType, ServiceInfoBase[] services)
            : base(id, name, description, sysName, modulePartType)
        {
            this.services = services;
            if (services != null)
                foreach (ServiceInfoBase srvInfo in services)
                    srvInfo.ModulePartInfo = this;
        }

        #region IModuleServicesPartInfo Members

        public IServiceInfo[] Services
        {
            get { return Array.ConvertAll<ServiceInfoBase, IServiceInfo>(services, si => si); }
        }

        #endregion
    }
}