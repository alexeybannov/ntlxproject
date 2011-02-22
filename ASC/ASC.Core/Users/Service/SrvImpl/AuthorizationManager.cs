using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Factories;
using ASC.Core.Users.DAO;
using UserConst = ASC.Core.Users.Constants;

[assembly: AssemblyServices(typeof(ASC.Core.Users.Service.SrvImpl.AuthorizationManager))]

namespace ASC.Core.Users.Service.SrvImpl
{
	class AuthorizationManager : RemotingServiceController, IAuthorizationManager
	{
		private IDAOFactory factory;

		internal AuthorizationManager(IDAOFactory daoFactory)
			: base(UserConst.AuthorizationManagerServiceInfo)
		{
			if (daoFactory == null) throw new ArgumentNullException("daoFactory");
			this.factory = daoFactory;
		}

		#region IAuthorizationManager

		///<inheritdoc/>
		public AzRecord[] GetAces()
		{
			return GetAzDAO().GetAce(null, null, null).ToArray();
		}

		///<inheritdoc/>
		[PrincipalPermission(SecurityAction.Demand, Role = Role.Administrators)]
		[PrincipalPermission(SecurityAction.Demand, Role = Role.System)]
		public void AddAce(AzRecord azRecord)
		{
			if (azRecord.Inherited) throw new InvalidOperationException("Can not add inherited authorization record");
			SecurityContext.DemandPermissions(UserConst.Action_EditAz);

			GetAzDAO().SaveAce(azRecord);
		}

		///<inheritdoc/>
		[PrincipalPermission(SecurityAction.Demand, Role = Role.Administrators)]
		[PrincipalPermission(SecurityAction.Demand, Role = Role.System)]
		public void RemoveAce(AzRecord azRecord)
		{
			if (azRecord.Inherited) throw new InvalidOperationException("Can not remove inherited authorization record");
			SecurityContext.DemandPermissions(UserConst.Action_EditAz);

			GetAzDAO().RemoveAce(azRecord);
		}

		#endregion

		private IAzDAO GetAzDAO()
		{
			return factory.GetAzDao();
		}
	}
}