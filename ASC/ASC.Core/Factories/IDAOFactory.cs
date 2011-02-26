using ASC.Core.Users.DAO;

namespace ASC.Core.Factories
{
	interface IDAOFactory
	{
		IAzDAO GetAzDao();

		ISubscriptionDAO GetSubscriptionDao();
	}
}