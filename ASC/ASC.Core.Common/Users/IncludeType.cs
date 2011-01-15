#region usings

using System;

#endregion

namespace ASC.Core.Users
{
    [Flags]
    public enum IncludeType
    {
        Distinct = 0x01,

        InParent = 0x02,

        InChild = 0x04,

        All = Distinct | InParent | InChild
    }
}