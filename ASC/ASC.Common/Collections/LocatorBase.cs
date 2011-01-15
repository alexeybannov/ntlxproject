#region usings

using System;
using System.Collections.Generic;
using ASC.Reflection;

#endregion

namespace ASC.Collections
{
    public class LocatorBase<keyT, instanceT> : ILocator<keyT, instanceT> where instanceT : class
    {
        private readonly IDictionary<keyT, Type> _Registry = new Dictionary<keyT, Type>();

        #region ILocator<keyT,instanceT>

        public void Register(keyT key, Type instanceType)
        {
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");
            if (!TypeHelper.IsSameOrParent(typeof (instanceT), instanceType))
                throw new ArgumentException(
                    String.Format(
                        "Type \"{0}\" not implement type \"{1}\"",
                        instanceType,
                        typeof (instanceT)
                        )
                    );

            if (!_Registry.ContainsKey(key))
                _Registry.Add(key, instanceType);
            else
                _Registry[key] = instanceType;
        }

        public void UnRegister(keyT key)
        {
            if (!_Registry.ContainsKey(key))
                _Registry.Remove(key);
        }

        public Type GetType(keyT key)
        {
            if (!_Registry.ContainsKey(key))
                return null;
            return _Registry[key];
        }

        public instanceT CreateInstance(keyT key)
        {
            if (!_Registry.ContainsKey(key))
                return null;
            var obj = CreateInstance(key, _Registry[key]) as instanceT;
            if (obj == null)
                obj = TypeInstance.Create(_Registry[key]) as instanceT;
            return obj;
        }

        public instanceT CreateInstance(keyT key, params object[] ctorParams)
        {
            if (!_Registry.ContainsKey(key))
                return null;
            var obj = TypeInstance.Create(_Registry[key], ctorParams) as instanceT;
            return obj;
        }

        #endregion

        protected virtual object CreateInstance(keyT key, Type T)
        {
            return null;
        }
    }
}