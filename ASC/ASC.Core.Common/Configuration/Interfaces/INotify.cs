using ASC.Common.Services;
using ASC.Notify.Engine;
using ASC.Runtime.Remoting.Channels;

namespace ASC.Core.Configuration
{
    [Service("{47E82D68-7D20-4fb3-8459-5B8F765EA7AF}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    public interface INotify : IService, INotifyDispatcher
    {
    }
}