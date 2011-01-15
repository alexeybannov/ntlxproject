#region usings

using System;
using ASC.Common.Module;
using ASC.Net;

#endregion

namespace ASC.Common.Services
{
    public interface IServiceInfo : IBaseInfo
    {
        string ServiceTypeName { get; }

        Type ServiceType { get; }

        IModulePartInfo ModulePartInfo { get; }

        ServiceInstancingType InstancingType { get; }

        TransportType[] TransportTypes { get; }

        string Uri { get; }
    }
}