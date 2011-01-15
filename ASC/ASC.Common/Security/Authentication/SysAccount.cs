#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public class SysAccount : AccountBase, ISysAccount
    {
        public SysAccount(Guid id, string name)
            : base(id, name)
        {
        }
    }
}