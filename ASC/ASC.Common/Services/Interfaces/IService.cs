#region usings

using ASC.Common.Security;
using ASC.Common.Security.Authentication;

#endregion

namespace ASC.Common.Services
{
    public interface IService
    {
        #region information part

        IServiceInfo Info { [AuthenticationLevel(SecurityLevel.None)]
        get; }

        #endregion
    }
}