using System.Configuration;
using ASC.Core.Configuration;

namespace ASC.Core
{
    public static class CoreContext
    {
        static CoreContext()
        {
            var tenantService = new DbTenantService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var quotaService = new DbQuotaService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var userService = new DbUserService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var azService = new DbAzService(ConfigurationManager.ConnectionStrings["core_nc"]);
            var subService = new DbSubscriptionService(ConfigurationManager.ConnectionStrings["core_nc"]);

            Configuration = new ClientConfiguration(tenantService);
            TenantManager = new ClientTenantManager(tenantService, quotaService);
            UserManager = new ClientUserManager(userService);
            Authentication = new ClientAuthManager(userService);
            AuthorizationManager = new ClientAzManager(azService);
            SubscriptionManager = new ClientSubscriptionManager(subService);
        }


        public static IClientConfiguration Configuration
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

        public static IAuthManagerClient AuthorizationManager
        {
            get;
            private set;
        }

        public static INotify Notify
        {
            get { return null; }
        }

        internal static ClientSubscriptionManager SubscriptionManager
        {
            get;
            private set;
        }
    }
}