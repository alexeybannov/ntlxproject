using ASC.Core.Configuration.DAO;
using ASC.Core.Users.DAO;

namespace ASC.Core.Factories
{
	public interface IDAOFactory
	{
		IAzDAO GetAzDao();

		ISubscriptionDAO GetSubscriptionDao();

		ICfgDAO GetConfigDao();

		ITenantDAO GetTenantDAO();
	}
}