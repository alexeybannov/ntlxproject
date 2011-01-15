using System.Collections.Generic;

namespace ASC.Core.Common.Cache
{
    public interface ICache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        string Id
        {
            get;
        }

        void Synchronize();

        void Flush();
    }
}