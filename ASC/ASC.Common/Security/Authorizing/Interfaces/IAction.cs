#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public interface IAction
    {
        Guid ID { get; }

        string Name { get; }

        string Description { get; }

        string SysName { get; }
    }
}