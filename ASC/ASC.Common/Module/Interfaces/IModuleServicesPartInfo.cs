#region usings

using ASC.Common.Services;

#endregion

namespace ASC.Common.Module
{
    public interface IModuleServicesPartInfo : IModulePartInfo
    {
        IServiceInfo[] Services { get; }
    }
}