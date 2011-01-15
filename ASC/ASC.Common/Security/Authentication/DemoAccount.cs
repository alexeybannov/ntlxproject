#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public class DemoAccount : SysAccount
    {
        public DemoAccount()
            : base(new Guid(CommonDescriptionResource.SysRole_Demo_ID), CommonDescriptionResource.SysRole_Demo_Name)
        {
        }
    }
}