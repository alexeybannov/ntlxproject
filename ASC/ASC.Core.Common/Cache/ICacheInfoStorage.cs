using ASC.Common.Security;
using ASC.Common.Security.Authentication;

namespace ASC.Core.Common.Cache
{
    public interface ICacheInfoStorage
    {
        [AuthenticationLevel(SecurityLevel.None)]
        void RegisterCache(CacheInfo info);

        [AuthenticationLevel(SecurityLevel.None)]
        CacheInfoStorageResult ValidateCache(CacheVersion version);

        [AuthenticationLevel(SecurityLevel.None)]
        CacheInfoStorageResult UpdateCache(CacheVersion version, CacheAction action);
    }
}