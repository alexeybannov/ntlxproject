using System;
using System.Collections;
using System.Configuration;
using System.Web.Configuration;
using ASC.Common.Data;
using ASC.Common.Services;
using ASC.Common.Services.Factories;
using ASC.Core.Configuration;
using ASC.Core.Configuration.Module;
using ASC.Core.Configuration.Service;
using ASC.Core.Users;
using ASC.Core.Users.Service.SrvImpl;

namespace ASC.Core.Factories
{
    public class EmbedCoreServiceFactory : IServiceFactory
    {
        readonly IDAOFactory daoFactory;

        public EmbedCoreServiceFactory()
        {
            var _srvPart = new CommonServicesModulePart();
            daoFactory = new DAOFactory();

            if (!DbRegistry.IsDatabaseRegistered(DAOFactory.DAO_KEY))
            {
                DbRegistry.RegisterDatabase(DAOFactory.DAO_KEY, WebConfigurationManager.ConnectionStrings["core"]);
            }

            string propertiesStr = ConfigurationManager.AppSettings["workcontextproperties"];
            if (!string.IsNullOrEmpty(propertiesStr))
            {
                foreach (var propertyStr in propertiesStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] nameValue = propertyStr.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameValue.Length == 2) WorkContext.SetProperty(nameValue[0], nameValue[1]);
                }
            }
        }

        #region IServiceFactory

        public IService GetService(Type type)
        {
            var result = GetOrCreateService(type);
            if (result == null)
            {
                throw new ArgumentException(String.Format("type {0} not supported by this factory", type.FullName), "type");
            }
            return result;
        }
        #endregion

        readonly Hashtable _services = Hashtable.Synchronized(new Hashtable(10));

        private IService GetOrCreateService(Type type)
        {

            var result = (IService)_services[type];
            if (result == null)
            {
                lock (_services.SyncRoot)
                {
                    result = (IService)_services[type];
                    if (result == null)
                    {
                        if (type == typeof(IAuthorizationManager))
                            result = new AuthorizationManager(daoFactory);
                        else if (type == typeof(ISubscriptionManager))
                            result = new SubscriptionManager(daoFactory);
                        else if (type == typeof(IAuthentication))
                            result = new Authentication(daoFactory);
                        else if (type == typeof(IServiceLocator))
                            result = new ServiceLocator();
                        else if (type == typeof(INotify))
                            result = new NotifyImpl();
                        else if (type == typeof(IConfiguration))
                            result = new ASC.Core.Configuration.Service.Configuration(daoFactory);
                        else if (type == typeof(ICacheInfoStorageService))
                            result = new CacheInfoStorageService();
                        else if (type == typeof(ITenantManager))
                            result = new TenantManager(daoFactory);

                        if (result != null)
                            _services[type] = result;
                    }
                }
            }

            return result;
        }
    }
}
