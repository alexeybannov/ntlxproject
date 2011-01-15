#region usings

using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Core.Common.Cache;

#endregion

namespace ASC.Core.Configuration
{
    [Service("{3B56C8C2-401B-43b9-84DC-00858650B1E9}")]
    [AuthenticationLevel(SecurityLevel.None)]
    public interface ICacheInfoStorageService : ICacheInfoStorage, IService
    {
    }
}