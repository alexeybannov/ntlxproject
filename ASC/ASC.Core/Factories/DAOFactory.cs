using ASC.Core.Users.DAO;

namespace ASC.Core.Factories
{
    class DAOFactory : IDAOFactory
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

        #endregion

        private int GetTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}