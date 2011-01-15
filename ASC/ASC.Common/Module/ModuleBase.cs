#region usings

using System;
using ASC.Common.Security.Authorizing;
using ASC.Common.Utils;

#endregion

namespace ASC.Common.Module
{
    public abstract class ModuleBase : IModule
    {
        protected IModuleInfo _ModuleInfo;
        protected IModulePart[] _ModuleParts;

        protected ModuleBase(IModuleInfo info, IModulePart[] parts)
        {
            if (info == null) throw new ArgumentNullException("info");
            _ModuleInfo = info;
            _ModuleParts = ArrayUtil.CopyClonable(parts);
        }

        protected ModuleBase(IModuleInfo info) : this(info, null)
        {
        }

        #region IModule Members

        public IModuleInfo Info
        {
            get { return _ModuleInfo; }
        }

        public IModulePart[] Parts
        {
            get { return ArrayUtil.CopyClonable(_ModuleParts); }
        }

        public virtual AuthCategory[] AuthorizingCategories
        {
            get { return new AuthCategory[0]; }
        }

        #endregion
    }
}