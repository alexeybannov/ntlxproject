#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public interface IServiceActivator
    {
        void RegisterForLocalInvoke(IService service);

        void UnRegisterForLocalInvoke(IService service);

        T Activate<T>() where T : IService;

        T Activate<T>(Guid serviceInstanceID) where T : IInstancedService;

        IService Activate(Guid ServiceID);

        IInstancedService Activate(Guid ServiceID, Guid serviceInstanceID);
    }
}