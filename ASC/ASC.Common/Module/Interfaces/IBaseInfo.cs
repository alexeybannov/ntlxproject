#region usings

using System;

#endregion

namespace ASC.Common.Module
{
    public interface IBaseInfo
    {
        Guid ID { get; }

        string Name { get; }

        string Description { get; }
        string SysName { get; }

        Version Version { get; }
    }
}