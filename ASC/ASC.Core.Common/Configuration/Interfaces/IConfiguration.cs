#region usings

using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Configuration
{
    [Service("{37017EA8-7826-43b6-BDF3-D5A02AE80C69}", ServiceInstancingType.Singleton)]
    [AuthenticationLevel(SecurityLevel.None)]
    interface IConfiguration : IService
    {
        int SecureCorePort { get; }

        bool Standalone { get; }

        void SaveSetting(string key, object value);

        object GetSetting(string key);
    }
}