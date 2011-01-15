#region usings

using System;
using System.Net;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Common.Security.Authentication
{
    [Serializable]
    public class ServiceAccount : AccountBase, IServiceAccount
    {
        public string HostName { get; set; }

        public Guid InstanceID { get; set; }

        public Guid ModuleID { get; set; }

        public Guid ServiceID { get; set; }

        public ServiceAccount(Guid id, string name)
            : base(id, name)
        {
        }

        public static ServiceAccount CreateFor(IService service)
        {
            Guid serviceID = service.Info.ID;
            Guid accountID = serviceID;
            Guid serviceInstanceID = Guid.Empty;
            if (service is IInstancedService)
            {
                serviceInstanceID = ((IInstancedService) service).InstanceID;
                accountID = serviceInstanceID;
            }
            string hostName = Dns.GetHostName();
            string name = service.Info.SysName;
            Guid moduleID = service.Info.ModulePartInfo.ModuleInfo.ID;
            return new ServiceAccount(accountID, name)
                       {
                           ServiceID = serviceID,
                           InstanceID = serviceInstanceID,
                           HostName = hostName,
                           ModuleID = moduleID,
                       };
        }

        public override string ToString()
        {
            return string.Format("Service: \"{0}\" on [{1}]", Name, HostName, SecurityLevel);
        }
    }
}