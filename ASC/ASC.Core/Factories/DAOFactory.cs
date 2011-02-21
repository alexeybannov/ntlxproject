using ASC.Core.Configuration.DAO;
using ASC.Core.Users.DAO;

namespace ASC.Core.Factories
{
    public class DAOFactory : IDAOFactory
    {
        public static readonly string DAO_KEY = "DAO_KEY";

        public DAOFactory()
        {

        }

        #region IDAOFactory

        ///<inheritdoc />
        public IAzDAO GetAzDao()
        {
            return new AzDAO(DAO_KEY, GetTenant());
        }

        ///<inheritdoc />
        public ISubscriptionDAO GetSubscriptionDao()
        {
            return new SubscriptionDAO(DAO_KEY, GetTenant());
        }

        /// <inheritdoc/>
        public ICfgDAO GetConfigDao()
        {
            return new CfgDAO(DAOFactory.DAO_KEY);
        }

        /// <inheritdoc/>
        public ITenantDAO GetTenantDAO()
        {
            return new TenantDAO(DAOFactory.DAO_KEY);
        }

        #endregion

        private int GetTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}