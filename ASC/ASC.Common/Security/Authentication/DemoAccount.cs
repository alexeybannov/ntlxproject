#region usings

using System;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public class DemoAccount : SysAccount
    {
        public DemoAccount()
            : base(Constants.Demo.ID, Constants.Demo.Name)
        {
        }
    }
}