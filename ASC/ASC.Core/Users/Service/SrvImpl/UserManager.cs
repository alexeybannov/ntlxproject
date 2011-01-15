using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Factories;
using ASC.Core.Users.DAO;
using ASC.Core.Users.Service.SrvImpl;
using log4net;

[assembly: AssemblyServices(typeof(UserManager))]

namespace ASC.Core.Users.Service.SrvImpl
{
	class UserManager : RemotingServiceController, IUserManager
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(UserManager));

		private IDAOFactory daoFactory;

		private IDictionary<Guid, byte[]> photosCache = new Dictionary<Guid, byte[]>();


		internal UserManager(IDAOFactory daoFactory)
			: base(Constants.UserManagerServiceInfo)
		{

			if (daoFactory == null) throw new ArgumentNullException("daoFactory");
			this.daoFactory = daoFactory;
		}

		#region IUserManager Members

		/// <inheritdoc />
		public UserInfo[] GetUsers()
		{
			try
			{
				return GetUserGroupDAO().GetUsers().ToArray();
			}
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_LoadError, exc);
			}
		}

		/// <inheritdoc />
		public UserInfo GetUsers(Guid userID)
		{
			try
			{
				var u = GetUserGroupDAO().GetUser(userID);
				if (u == null) throw new UserNotFoundException(userID);
				return u;
			}
			catch (UserManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_LoadError, exc);
			}
		}

		/// <inheritdoc />
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public UserInfo SaveUserInfo(UserInfo userInfo)
		{
			if (userInfo == null) throw new ArgumentNullException("userInfo");
			try
			{
				if (userInfo.ID == Guid.Empty)
				{
					SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);
				}
				else
				{
					SecurityContext.DemandPermissions<UserInfo>(userInfo.ID, new UserSecurityProvider(), Constants.Action_EditUser);
				}

				return GetUserGroupDAO().SaveUser(userInfo);
			}
			catch (AuthorizingException) { throw; }
			catch (UserManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_SaveError, exc);
			}
		}

		/// <inheritdoc />
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void DeleteUser(Guid userID)
		{
			if (userID == Guid.Empty) return;
			SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);
			try
			{
				GetUserGroupDAO().RemoveUser(userID);
			}
			catch (UserManipulationException)
			{
				throw;
			}
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_DeleteError, exc);
			}
		}

		/// <inheritdoc />
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo)
		{
			if (SecurityContext.CurrentAccount.ID == userID && moduleID != Guid.Empty)
			{
				//can change photo in any modules exept main module
			}
			else
			{
				SecurityContext.DemandPermissions<UserInfo>(userID, new UserSecurityProvider(), Constants.Action_EditUser);
			}

			try
			{
				GetUserGroupDAO().SaveUserPhoto(userID, moduleID, photo);
				photosCache[userID] = photo;
			}
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_SaveError, exc);
			}
		}

		/// <inheritdoc />
		public byte[] GetUserPhoto(Guid userID, Guid moduleID)
		{
			try
			{
				if (!photosCache.ContainsKey(userID))
				{
					photosCache[userID] = GetUserGroupDAO().GetUserPhoto(userID, moduleID);
				}
				return photosCache[userID];
			}
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_SaveError, exc);
			}
		}

		/// <inheritdoc />
		public UserGroupReference[] GetUsersGroupReferences()
		{
			return GetUserGroupDAO().GetUserGroupRefs().ToArray();
		}

		/// <inheritdoc />
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void AddUserIntoGroup(Guid userID, Guid groupID)
		{
			try
			{
				var gi = CoreContext.GroupManager.GetGroupInfo(groupID);
				if (gi != null)
				{
					GroupSecurityHelper.DemandPermission(gi);
					GetUserGroupDAO().SaveUserGroupRef(userID, groupID);
				}
			}
			catch (AuthorizingException) { throw; }
			catch (UserAllreadyInGroupException) { throw; }
			catch (GroupManipulationException) { throw; }
			catch (UserManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_LoadError, exc);
			}
		}

		/// <inheritdoc />
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void RemoveUserFromGroup(Guid userID, Guid groupID)
		{
			try
			{
				var g = CoreContext.GroupManager.GetGroupInfo(groupID);
				if (g != null)
				{
					GroupSecurityHelper.DemandPermission(g);
					GetUserGroupDAO().RemoveUserGroupRef(userID, groupID);
				}
			}
			catch (AuthorizingException) { throw; }
			catch (GroupManipulationException) { throw; }
			catch (UserManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateUserManipulationException(DescriptionResource.UME_LoadError, exc);
			}
		}

		/// <inheritdoc/>
		public CompanyProperty[] GetCompanyProperties()
		{
			return GetCompanyDAO().GetCompanyProperties().ToArray();
		}

		/// <inheritdoc/>
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void SetCompanyProperty(string propertyName, string data)
		{
			if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
			GetCompanyDAO().SaveCompanyProperty(new CompanyProperty(propertyName, data));
		}

		/// <inheritdoc/>
		public DepartmentManagerRef[] GetDepartmentManagerRefs()
		{
			return new List<DepartmentManagerRef>(GetCompanyDAO().GetDepartmentManagerRefs()).ToArray();
		}

		/// <inheritdoc/>
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void SetDepartmentManager(Guid deparmentID, Guid userID)
		{
			GetCompanyDAO().SetDepartmentManager(deparmentID, userID);
		}

		/// <inheritdoc/>
		public bool RemoveDepartmentManager(Guid deparmentID, Guid userID)
		{
			return GetCompanyDAO().RemoveDepartmentManager(deparmentID, userID);
		}

		#endregion

		private IUserGroupDAO GetUserGroupDAO()
		{
			return daoFactory.GetUserGroupDAO();
		}

		private ICompanyDAO GetCompanyDAO()
		{
			return daoFactory.GetCompanyDao();
		}

		private UserManipulationException CreateUserManipulationException(string message, Exception exc)
		{
			if (exc != null) log.Error("CreateUserManipulationException", exc);
			return new UserManipulationException(message, exc != null ? exc.ToString() : null);
		}
	}
}