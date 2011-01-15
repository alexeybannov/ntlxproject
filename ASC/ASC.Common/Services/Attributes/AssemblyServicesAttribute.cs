#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyServicesAttribute : Attribute
    {
        private readonly Type[] _ServicesTypes;

        public AssemblyServicesAttribute(params Type[] servicesTypes)
        {
            if (servicesTypes == null)
                _ServicesTypes = new Type[0];
            else
                _ServicesTypes = servicesTypes;
        }

        public Type[] ServicesTypes
        {
            get { return _ServicesTypes; }
        }
    }
}