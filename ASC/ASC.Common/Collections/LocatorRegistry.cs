#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Collections
{
    public abstract class LocatorRegistry<keyT, instanceT> where instanceT : class
    {
        private readonly IList<ILocator<keyT, instanceT>> _Locators = new List<ILocator<keyT, instanceT>>();

        public Type GetType(keyT key)
        {
            Type type = null;
            foreach (var locator in _Locators)
            {
                type = locator.GetType(key);
                if (type != null) break;
            }
            return type;
        }

        public instanceT CreateInstance(keyT key)
        {
            instanceT instance = null;
            foreach (var locator in _Locators)
            {
                instance = locator.CreateInstance(key);
                if (instance != null) break;
            }
            return instance;
        }

        public instanceT CreateInstance(keyT key, params object[] ctorParams)
        {
            instanceT instance = null;
            foreach (var locator in _Locators)
            {
                instance = locator.CreateInstance(key, ctorParams);
                if (instance != null) break;
            }
            return instance;
        }

        public void RegisterServiceLocator(ILocator<keyT, instanceT> serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            lock (_Locators)
            {
                _Locators.Add(serviceLocator);
            }
        }

        public void UnRegisterServiceLocator(ILocator<keyT, instanceT> serviceLocator)
        {
            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");
            lock (_Locators)
            {
                _Locators.Remove(serviceLocator);
            }
        }

        protected ILocator<keyT, instanceT> GetLocatorByType(Type locatorType)
        {
            foreach (var locator in _Locators)
                if (locator.GetType() == locatorType) return locator;
            return null;
        }
    }
}