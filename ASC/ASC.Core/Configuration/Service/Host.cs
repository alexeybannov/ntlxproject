using System;
using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Core.Common.Services;
using ASC.Core.Factories;

namespace ASC.Core.Configuration.Service
{
	class Host : ServiceHost
	{
		private IDAOFactory daoFactory = new DAOFactory();

		internal Host(IModuleServicesPart servicePart)
			: base("common configuration services host", servicePart)
		{
		}

		protected override IServiceController CreateService(IServiceInfo srvInfo)
		{
			if (srvInfo == null) throw new ArgumentNullException("srvInfo");

			if (srvInfo.ID == Constants.ServiceLocatorServiceInfo.ID)
				return new ServiceLocator();

			return null;
		}
	}
}
