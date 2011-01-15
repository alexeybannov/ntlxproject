#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    [Flags]
    public enum ServiceStatus : uint
    {
        Unknown = 0x0000,

        Sys_FFFF0000 = 0xFFFF0000,

        Sys_FF00 = 0xFF00,

        Sys_00FF = 0x00FF,

        Stopped = 0x0001,

        Running = 0x0002,

        StartPendind = 0x0004,

        StopPendind = 0x0008,

        Ok = 0x0100,

        Warning = 0x0200,

        Error = 0x0400
    }
}