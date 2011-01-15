using System.Collections.Generic;

namespace ASC.Core.Common.Cache
{
    public delegate IDictionary<TKey, TValue> SynchronizeFunc<TKey, TValue>();
}