#region usings

using System;

#endregion

namespace ASC.Core.Common.Cache
{
    [Flags]
    public enum CacheAction
    {
        Save = 1,

        Update = 2,

        Remove = 4,

        Flush = 8,

        SaveOrUpdate = Save | Update,

        SaveOrRemove = Save | Remove
    }
}