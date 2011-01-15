#region usings

using System;

#endregion

namespace ASC.Common.Module
{
    [Flags]
    public enum ModuleType : short
    {
        CoreInfrastructure = 0x01,
        HumanCommunication = 0x02,
        NetAdministration = 0x04,
        OfficeAdministration = 0x08
    }
}