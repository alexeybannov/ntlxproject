using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Core.Common.Services;
using ASC.Core.Factories;
using ASC.Core.Users.Service.SrvImpl;

namespace ASC.Core.Users.Service
{
	class Host : ServiceHost
	{
		private IDAOFactory daoFactory = null;

		internal Host()
			: base("user manager host")
		{

		}

        internal Host(IModuleServicesPart servicePart)
			: base("user manager host", servicePart)
		{

		}

		protected override IServiceController CreateService(IServiceInfo srvInfo)
		{
			if (srvInfo.ID == Constants.UserManagerServiceInfo.ID)
			{
				return new UserManager(DaoFactory);
			}
			else if (srvInfo.ID == Constants.GroupManagerServiceInfo.ID)
			{
				return new GroupManager(DaoFactory);
			}
			else if (srvInfo.ID == Constants.AuthorizationManagerServiceInfo.ID)
			{
				return new AuthorizationManager(DaoFactory);
			}
			else if (srvInfo.ID == Constants.SubscriptionManagerServiceInfo.ID)
			{
				return new SubscriptionManager(DaoFactory);
			}

			return base.CreateService(srvInfo);
		}

		internal IDAOFactory DaoFactory
		{
			get
			{
				if (daoFactory == null) daoFactory = new DAOFactory();
				return daoFactory;
			}
		}
	}
}