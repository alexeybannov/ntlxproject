#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    public interface IServiceAccount : IAccount
    {
        string HostName { get; }
        Guid ModuleID { get; }
        Guid ServiceID { get; }
        Guid InstanceID { get; }
    }
}