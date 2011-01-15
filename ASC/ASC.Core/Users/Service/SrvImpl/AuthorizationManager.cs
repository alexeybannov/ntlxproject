using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ASC.Common.Security;
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

		/// <inheritdoc/>
		public IList<AzObjectInfo> GetAzObjectInfos()
		{
			return GetAzDAO().GetAzObjectInfos();
		}

		/// <inheritdoc/>
		public void SaveAzObjectInfo(AzObjectInfo azObjectInfo)
		{
			SecurityContext.DemandPermissions(UserConst.Action_EditAz);
			GetAzDAO().SaveAzObjectInfo(azObjectInfo);
		}

		/// <inheritdoc/>
		public void RemoveAzObjectInfo(AzObjectInfo azObjectInfo)
		{
			SecurityContext.DemandPermissions(UserConst.Action_EditAz);
			GetAzDAO().RemoveAzObjectInfo(azObjectInfo);
		}

		/// <inheritdoc/>
		public AzRecord[] GetAces(Guid subjectID, Guid actionID, ISecurityObjectId objectId)
		{
			throw new NotSupportedException();
		}

		///<inheritdoc/>
		public AzCategory[] GetAzCategories(Guid moduleID)
		{
			throw new NotSupportedException();
		}

		///<inheritdoc/>
		public AzRecord[] GetAces(Guid subjectID, Guid actionID)
		{
			throw new NotSupportedException();
		}

		///<inheritdoc/>
		public AzRecord[] GetAcesBySubject(Guid subjectID)
		{
			throw new NotSupportedException();
		}

		///<inheritdoc/>
		public AzRecord[] GetAcesByAction(Guid actionID)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObject objectId)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzRecord[] GetAllObjectAces(IEnumerable<IAction> actions, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzRecord[] GetAllObjectAces<T>(IEnumerable<IAction> actions, object objectId, ISecurityObjectProvider secObjProvider)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public bool GetObjectAceInheritance(ISecurityObjectId objectId)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public void SetObjectAceInheritance(ISecurityObjectId objectId, bool inherit)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public bool GetObjectAceInheritance<T>(object objectId)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public void SetObjectAceInheritance<T>(object objectId, bool inherit)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzObjectInfo GetAzObjectInfo<T>(object objectId)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public AzObjectInfo GetAzObjectInfo(ISecurityObjectId objectId)
		{
			throw new NotSupportedException();
		}

		#endregion

		private IAzDAO GetAzDAO()
		{
			return factory.GetAzDao();
		}
	}
}