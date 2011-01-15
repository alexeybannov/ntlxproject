#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public interface IServicesModulePartsHost
        : IController
    {
        Guid[] ServicesModuleParts { get; }
        HostStatus Status { get; }
        void Start(Guid servicesModulePartID);

        void Stop(Guid servicesModulePartID);

        void Add(Guid servicesModulePartID);

        void Remove(Guid servicesModulePartID);

        HostStatus GetStatus(Guid servicesModulePartID);
    }
}