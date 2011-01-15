#region usings

using System;
using ASC.Common.Utils;

#endregion

namespace ASC.Common.Module
{
    [Serializable]
    public class ModuleInfoBase : IModuleInfo
    {
        private readonly string _Description;
        private readonly Guid _ID;
        private readonly ModulePartInfoBase[] _ModulePartsInfo;
        private readonly string _Name;
        private readonly string _SysName;
        private readonly ModuleType _Type;
        private readonly Version _Version;

        public ModuleInfoBase(Guid id, string name, string description, string sysName, Version version, ModuleType type,
                              ModulePartInfoBase[] modulePartsInfo)
        {
            if (id == Guid.Empty) throw new ArgumentOutOfRangeException("id");
            _ID = id;
            _Name = name;
            _Description = description;
            SysNameChecker.Check(sysName);
            _SysName = sysName;
            if (version != null)
                _Version = (Version) version.Clone();
            else
                _Version = new Version();
            _Type = type;
            _ModulePartsInfo = ArrayUtil.CopyClonable(modulePartsInfo);
            if (_ModulePartsInfo != null)
                for (int i = 0; i < _ModulePartsInfo.Length; i++)
                    _ModulePartsInfo[i].ModuleInfo = this;
        }

        public virtual string ModuleCredentials { get; set; }

        #region IModuleInfo Members

        public ModuleType Type
        {
            get { return _Type; }
        }

        public IModulePartInfo[] ModulePartsInfo
        {
            get { return (IModulePartInfo[]) _ModulePartsInfo.Clone(); }
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
            get { return _Version; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var mi = obj as IModuleInfo;
            if (mi == null) return false;
            return Equals(ID, mi.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}