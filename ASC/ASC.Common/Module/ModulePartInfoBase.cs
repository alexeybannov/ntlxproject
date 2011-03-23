#region usings

using System;
using ASC.Common.Utils;

#endregion

namespace ASC.Common.Module
{
    [Serializable]
    public class ModulePartInfoBase
    {
        #region fields

        private readonly string _Description;
        private readonly Guid _ID;
        private readonly ModulePartType _ModulePartType;
        private readonly string _Name;
        private readonly string _SysName;
        private IModuleInfo _ModuleInfo;

        #endregion

        public ModulePartInfoBase(Guid id, string name, string description, string sysName,
                                  ModulePartType modulePartType)
        {
            if (id == Guid.Empty)
                throw new ArgumentOutOfRangeException("id");

            _ID = id;
            _Name = name;
            _Description = description;
            _SysName = sysName;
            _ModuleInfo = null;
            _ModulePartType = modulePartType;
        }

        #region IModulePartInfo Members

        public IModuleInfo ModuleInfo
        {
            get { return _ModuleInfo; }
            internal set { _ModuleInfo = value; }
        }

        public ModulePartType ModulePartType
        {
            get { return _ModulePartType; }
        }

        public Guid ID
        {
            get { return _ID; }
        }

        public string Name
        {
            get { return _Name; }
        }

        public string Description
        {
            get { return _Description; }
        }

        public string SysName
        {
            get { return _SysName; }
        }

        public Version Version
        {
            get { return ModuleInfo != null ? ModuleInfo.Version : new Version(); }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var mpi = obj as ModulePartInfoBase;
            if (mpi == null) return false;
            return Equals(ID, mpi.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            if (ModuleInfo != null)
                return String.Format("{0}:{1}[{2}]", ModuleInfo.Name, Name, ModulePartType);
            else
                return String.Format("{0}[{1}]", Name, ModulePartType);
        }
    }
}