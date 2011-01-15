#region usings

using System;

#endregion

namespace ASC.Common.Module
{
    public abstract class ModulePartBase : IModulePart
    {
        private readonly IModulePartInfo _ModulePartInfo;

        protected ModulePartBase(IModulePartInfo partInfo)
        {
            if (partInfo == null)
                throw new ArgumentNullException("partInfo");
            _ModulePartInfo = partInfo;
        }

        #region IModulePart Members

        public IModulePartInfo Info
        {
            get { return _ModulePartInfo; }
        }

        #endregion
    }
}