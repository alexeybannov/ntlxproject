#region usings

using System;

#endregion

namespace ASC.Core.Users
{
    [Flags]
    public enum GroupType
    {
        LogicalGroup = 1,

        AuthorizeGroup = 2,
    }
}