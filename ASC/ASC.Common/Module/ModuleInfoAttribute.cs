#region usings

using System;
using ASC.Reflection;

#endregion

namespace ASC.Common.Module
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class ModuleInfoAttribute : Attribute
    {
        private readonly Guid _ModulePartID = Guid.Empty;
        private readonly Type _ModuleType;
        private readonly Version _Version = new Version(0, 0);

        #region Constructors

        public ModuleInfoAttribute(Type moduleType, string modulePartID, string version)
        {
            if (null == moduleType)
                throw new ArgumentNullException("moduleID");
            if (null == modulePartID)
                throw new ArgumentNullException("modulePartID");
            if (null == version)
                throw new ArgumentNullException("version");
            if (!TypeHelper.ImplementInterface(moduleType, typeof (IModule)))
                throw new ArgumentException(
                    String.Format(
                        "Type \"{0}\" not implements interface IModule",
                        moduleType
                        )
                    );
            _ModuleType = moduleType;
            _ModulePartID = new Guid(modulePartID);
            _Version = new Version(version);
        }

        #endregion

        public Type ModuleType
        {
            get { return _ModuleType; }
        }

        public Guid ModulePartID
        {
            get { return _ModulePartID; }
        }

        public Version Version
        {
            get { return _Version; }
        }
    }
}