using System;

namespace ASC.Common.Module
{
    public interface IModuleInfo
    {
        Guid ID { get; }

        string Name { get; }

        string Description { get; }
        string SysName { get; }

        Version Version { get; }

        ModuleType Type { get; }
        
        ModulePartInfoBase[] ModulePartsInfo { get; }
    }
}