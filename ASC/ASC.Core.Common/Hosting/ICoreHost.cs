#region usings

using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Hosting
{
    [Service("{2C561D10-CFC2-4fbb-9614-0C561EBB29D6}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    public interface ICoreHost : IService, IServicesModulePartsHost
    {
    }
}