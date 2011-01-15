#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    [Flags]
    public enum HostStatus
    {
        Unknown = 0x000,

        Stopped = 0x001,

        RunningSome = 0x002,

        RunningAll = 0x004,

        WarningSome = 0x010,

        WarningAll = 0x020,

        ErrorSome = 0x100,

        ErrorAll = 0x200
    }
}