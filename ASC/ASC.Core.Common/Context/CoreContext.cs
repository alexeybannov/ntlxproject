using System.Configuration;
using ASC.Common.Services;
using ASC.Core.Configuration;

namespace ASC.Core
{
    public static class CoreContext
    {
        static CoreContext()
        {
            var userService = new DbUserService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var tenantService = new DbTenantService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var quotaService = new DbQuotaService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var azService = new DbAzService(ConfigurationManager.ConnectionStrings["core_nc"]);

            Configuration = new ClientConfiguration(tenantService);
            TenantManager = new ClientTenantManager(tenantService, quotaService);
            UserManager = new ClientUserManager(userService);
            Authentication = new AuthenticationService(userService);
            AuthorizationManager = new AzClientManager(azService);
        }


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
            get;
            private set;
        }

        public static INotify Notify
        {
            get { return GetService<INotify>(); }
        }

        public static IServiceLocator ServiceLocator
        {
            get { return GetService<IServiceLocator>(); }
        }

        
        private static T GetService<T>()
            where T : IService
        {
            return WorkContext.GetService<T>();
        }
    }
}