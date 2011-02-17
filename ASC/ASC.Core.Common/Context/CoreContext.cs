#region usings

using System;
using ASC.Common.Services;
using ASC.Core.Common.Cache;
using ASC.Core.Configuration;
using ASC.Core.Users;

#endregion

namespace ASC.Core
{
    public static class CoreContext
    {
        #region << Private fields >>

        private static readonly object syncRoot;
        private static readonly ClientConfiguration configuration;
        private static Lazy<CacheInfoStorageClient> cacheInfoStorage;
        private static Lazy<ITenantManagerClient> tenantManager;
        private static Lazy<ClientUserManager> userManager;
        private static Lazy<AuthenticationService> authentication;
        private static Lazy<AzClientManager> azManager;
        private static Lazy<ClientSubscriptionManager> subscriptionManager;

        #endregion << Private fields >>

        static CoreContext()
        {
            syncRoot = new object();
            configuration = new ClientConfiguration();
            Reconnect();
        }

        internal static void Reconnect()
        {
            cacheInfoStorage = new Lazy<CacheInfoStorageClient>(() => new CacheInfoStorageClient(GetService<ICacheInfoStorageService>(), TimeSpan.FromSeconds(2)));
            tenantManager = new Lazy<ITenantManagerClient>(() => new ClientTenantManager());
            userManager = new Lazy<ClientUserManager>(() => new ClientUserManager(null));
            authentication = new Lazy<AuthenticationService>(() => new AuthenticationService());
            azManager = new Lazy<AzClientManager>(() => new AzClientManager());
            subscriptionManager = new Lazy<ClientSubscriptionManager>(() => new ClientSubscriptionManager(GetService<ISubscriptionManager>()));
        }

        #region << Public Properties >>

        public static CacheInfoStorageClient CacheInfoStorage
        {
            get { return cacheInfoStorage.Instance; }
        }

        public static ITenantManagerClient TenantManager
        {
            get { return tenantManager.Instance; }
        }

        public static IUserManagerClient UserManager
        {
            get { return userManager.Instance; }
        }

        public static IGroupManagerClient GroupManager
        {
            get { return userManager.Instance; }
        }

        public static IAuthenticationClient Authentication
        {
            get { return authentication.Instance; }
        }

        public static IAuthorizationManagerClient AuthorizationManager
        {
            get { return azManager.Instance; }
        }

        public static INotify Notify
        {
            get { return GetService<INotify>(); }
        }

        public static ISubscriptionManagerClient SubscriptionManager
        {
            get { return subscriptionManager.Instance; }
        }

        public static ClientConfiguration Configuration
        {
            get { return configuration; }
        }

        public static IServiceLocator ServiceLocator
        {
            get { return GetService<IServiceLocator>(); }
        }

        #endregion << Public Properties >>

        #region << Internal Services >>

        internal static IConfiguration InternalConfiguration
        {
            get { return GetService<IConfiguration>(); }
        }

        internal static IAuthentication InternalAuthentication
        {
            get { return GetService<IAuthentication>(); }
        }

        internal static IUserManager InternalUserManager
        {
            get { return GetService<IUserManager>(); }
        }

        internal static IGroupManager InternalGroupManager
        {
            get { return GetService<IGroupManager>(); }
        }

        internal static IAuthorizationManager InternalAuthorizationManager
        {
            get { return GetService<IAuthorizationManager>(); }
        }

        internal static ITenantManager InternalTenantManager
        {
            get { return GetService<ITenantManager>(); }
        }

        #endregion << Internal Services >>

        private static T GetService<T>()
            where T : IService
        {
            return WorkContext.GetService<T>();
        }
    }
}