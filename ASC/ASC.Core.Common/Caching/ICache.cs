using System;

namespace ASC.Core.Caching
{
    interface ICache
    {
        object Get(string key);

        void Insert(string key, object value, TimeSpan sligingExpiration);

        void Insert(string key, object value, DateTime absolutExpiration);

        object Remove(string key);
    }
}
