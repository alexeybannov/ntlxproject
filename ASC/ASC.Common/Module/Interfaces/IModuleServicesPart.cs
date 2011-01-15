#region usings

using ASC.Common.Services;

#endregion

namespace ASC.Common.Module
{
    public interface IModuleServicesPart : IModulePart
    {
        IServiceInfo[] Services { get; }
        IServiceHost ServiceHost { get; }
        IServiceController CreateServiceInstance(IServiceInfo srvInfo);
    }
}