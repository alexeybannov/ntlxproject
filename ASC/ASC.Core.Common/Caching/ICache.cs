using System;

namespace ASC.Core.Caching
{
    interface ICache
    {
        object Get(string key);

        void Insert(string key, object value, TimeSpan sligingExpiration);

        object Remove(string key);
    }
}
