#region usings

using System;
using ASC.Reflection;

#endregion

namespace ASC.Common.Services
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LocatorAttribute : Attribute
    {
        private readonly Type _LocatorType;

        public LocatorAttribute() : this(typeof (Locator))
        {
        }

        public LocatorAttribute(Type locatorType)
        {
            if (locatorType == null)
                throw new ArgumentNullException("locatorType");
            if (!TypeHelper.IsSameOrParent(typeof (Locator), locatorType))
                throw new ArgumentException(
                    String.Format("Type \"{0}\" not inherits Locator type", locatorType)
                    );
            _LocatorType = locatorType;
        }

        public Type LocatorType
        {
            get { return _LocatorType; }
        }
    }
}