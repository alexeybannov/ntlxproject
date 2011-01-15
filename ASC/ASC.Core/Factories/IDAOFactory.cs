using ASC.Core.Configuration.DAO;
using ASC.Core.Users.DAO;

namespace ASC.Core.Factories
{
	public interface IDAOFactory
	{
		IUserGroupDAO GetUserGroupDAO();

		IAzDAO GetAzDao();

		ISubscriptionDAO GetSubscriptionDao();

		ICompanyDAO GetCompanyDao();

		ICfgDAO GetConfigDao();

		ITenantDAO GetTenantDAO();
	}
}