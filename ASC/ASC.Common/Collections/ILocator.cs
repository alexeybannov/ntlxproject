#region usings

using System;

#endregion

namespace ASC.Collections
{
    public interface ILocator<keyT, instanceT> where instanceT : class
    {
        void Register(keyT key, Type instanceType);
        void UnRegister(keyT key);
        Type GetType(keyT key);
        instanceT CreateInstance(keyT key);
        instanceT CreateInstance(keyT key, params object[] ctorParams);
    }
}