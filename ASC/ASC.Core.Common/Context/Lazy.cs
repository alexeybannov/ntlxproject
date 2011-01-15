#region usings

using System;

#endregion

namespace ASC.Core
{
    internal class Lazy<T>
    {
        private object locker = new object();
        private T instance;
        private readonly Func<T> ctor;

        public T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (this)
                    {
                        if (instance == null) instance = ctor();
                    }
                }
                return instance;
            }
        }

        public Lazy(Func<T> ctor)
        {
            this.ctor = ctor;
        }
    }
}