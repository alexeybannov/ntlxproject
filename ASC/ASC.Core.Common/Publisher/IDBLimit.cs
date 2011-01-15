#region usings

using System.Collections.Generic;

#endregion

namespace ASC.Core.Common.Publisher
{
    public interface IDBLimit
    {
        IDictionary<string, string> Filters { get; }
    }
}