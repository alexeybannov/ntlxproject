using System;

namespace ASC.Core
{
    class CacheItem<T>
    {
        public T Value
        {
            get;
            private set;
        }

        public bool Removed
        {
            get;
            private set;
        }

        public DateTime ModifiedOn
        {
            get;
            private set;
        }


        public CacheItem(T value, bool removed, DateTime modifiedOn)
        {
            if (value == null) throw new ArgumentNullException("value");

            Value = value;
            Removed = removed;
            ModifiedOn = modifiedOn;
        }


        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
