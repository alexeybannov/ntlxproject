using System.Configuration;
using ASC.Core.Caching;
using ASC.Core.Configuration;
using ASC.Core.Data;

namespace ASC.Core
{
    public static class CoreContext
    {
        static CoreContext()
        {
            var cs = ConfigurationManager.ConnectionStrings["core_nc"];
            var tenantService = new DbTenantService(cs);
            var quotaService = new DbQuotaService(cs);
            var userService = new CachedUserService(new DbUserService(cs));
            var azService = new DbAzService(cs);
            var subService = new DbSubscriptionService(cs);

            Configuration = new ClientConfiguration(tenantService);
            TenantManager = new ClientTenantManager(tenantService, quotaService);
            UserManager = new ClientUserManager(userService);
            GroupManager = new ClientUserManager(userService);
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
            get;
            private set;
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