#region usings

using System;
using ASC.Common.Module;
using ASC.Reflection;

#endregion

namespace ASC.Common.Services
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AssemblyServiceHostAttribute : Attribute
    {
        private Type _HostType;
        private Guid _ServiceModulePartID = Guid.Empty;

        public AssemblyServiceHostAttribute(Type hostType, string serviceModulePartID)
        {
            if (hostType == null)
                throw new ArgumentNullException("hostType");
            if (serviceModulePartID == null)
                throw new ArgumentNullException("serviceModulePartID");
            Instancing(hostType, new Guid(serviceModulePartID));
        }

        public AssemblyServiceHostAttribute(Type hostType)
        {
            if (hostType == null)
                throw new ArgumentNullException("hostType");
            var moduleInfoAttr =
                (ModuleInfoAttribute[]) hostType.Assembly.GetCustomAttributes(typeof (ModuleInfoAttribute), true);
            if (moduleInfoAttr.Length < 1)
                throw new ArgumentException(
                    String.Format("Assembly \"{0}\" hasn't attribute ModuleInfoAttribute", hostType.Assembly),
                    "hostType.Assembly"
                    );
            Instancing(hostType, moduleInfoAttr[0].ModulePartID);
        }

        public Guid ServiceModulePartID
        {
            get { return _ServiceModulePartID; }
        }

        public Type HostType
        {
            get { return _HostType; }
        }

        private void Instancing(Type hostType, Guid serviceModulePartID)
        {
            if (!TypeHelper.ImplementInterface(hostType, typeof (IServiceHost)))
                throw new ArgumentException(
                    String.Format("Type \"{0}\" not implements interface IServiceHost", hostType),
                    "hostType"
                    );
            _HostType = hostType;
            _ServiceModulePartID = serviceModulePartID;
        }
    }
}