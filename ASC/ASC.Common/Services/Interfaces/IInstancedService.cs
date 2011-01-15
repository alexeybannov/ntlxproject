#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public interface IInstancedService : IService
    {
        Guid InstanceID { get; }
    }
}