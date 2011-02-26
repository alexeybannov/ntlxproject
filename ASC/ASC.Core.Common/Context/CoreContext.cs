using System.Configuration;
using ASC.Common.Services;
using ASC.Core.Configuration;

namespace ASC.Core
{
    public static class CoreContext
    {
        #region << Private fields >>

        private static readonly Lazy<AzClientManager> azManager;

        #endregion << Private fields >>

        static CoreContext()
        {
            var userService = new DbUserService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var tenantService = new DbTenantService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var quotaService = new DbQuotaService(ConfigurationManager.ConnectionStrings["core_nc"]);

            Configuration = new ClientConfiguration(tenantService);
            TenantManager = new ClientTenantManager(tenantService, quotaService);
            UserManager = new ClientUserManager(userService);
            Authentication = new AuthenticationService(userService);
            azManager = new Lazy<AzClientManager>(() => new AzClientManager());
        }

        #region << Public Properties >>

        public static ClientConfiguration Configuration
        {
            get;
            private set;
        }

        public static ITenantManagerClient TenantManager
        {
            get;
            private set;
        }

        public static IUserManagerClient UserManager
        {
            get;
            private set;
        }

        public static IGroupManagerClient GroupManager
        {
            get { return (IGroupManagerClient)UserManager; }
        }

        public static IAuthenticationClient Authentication
        {
            get;
            private set;
        }

        internal static ClientSubscriptionManager SubscriptionManager
        {
            get;
            private set;
        }

        public static IAuthorizationManagerClient AuthorizationManager
        {
            get { return azManager.Instance; }
        }

        public static INotify Notify
        {
            get { return GetService<INotify>(); }
        }

        public static IServiceLocator ServiceLocator
        {
            get { return GetService<IServiceLocator>(); }
        }

        #endregion << Public Properties >>


        private static T GetService<T>()
            where T : IService
        {
            return WorkContext.GetService<T>();
        }
    }
}