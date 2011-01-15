#region usings

using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IDBTarget
    {
        int Priority { get; }

        IDictionary<string, string> Filters { get; }
    }
}