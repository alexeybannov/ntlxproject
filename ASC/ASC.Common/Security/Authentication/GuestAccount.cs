#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public class GuestAccount : SysAccount
    {
        public GuestAccount(Guid ID, string Name)
            : base(ID, Name)
        {
        }

        #region IGuestAccount Members

        public override bool IsAuthenticated
        {
            get { return false; }
        }

        #endregion
    }
}