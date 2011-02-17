using System;
using System.Security.Permissions;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Factories;
using ASC.Core.Users.DAO;
using ASC.Core.Users.Service.SrvImpl;
using log4net;

[assembly: AssemblyServices(typeof(GroupManager))]

namespace ASC.Core.Users.Service.SrvImpl
{
	class GroupManager : RemotingServiceController, IGroupManager
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(GroupManager));
		private readonly IDAOFactory daoFactory;

		internal GroupManager(IDAOFactory daoFactory)
			: base(Constants.GroupManagerServiceInfo)
		{
			if (daoFactory == null) throw new ArgumentNullException("daoFactory");
			this.daoFactory = daoFactory;
		}

		public GroupInfo[] GetGroups()
		{
			try
			{
				return GetUserGroupDAO().GetGroups().ToArray();
			}
			catch (GroupCategoryManipulationException) { throw; }
			catch (GroupManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateGroupMException(DescriptionResource.GME_LoadError, exc);
			}
		}

		public GroupInfo GetGroupInfo(Guid groupID)
		{
			try
			{
				return GetUserGroupDAO().GetGroup(groupID);
			}
			catch (GroupManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateGroupMException(DescriptionResource.GME_LoadError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public GroupInfo SaveGroupInfo(GroupInfo groupInfo)
		{
			if (groupInfo == null) throw new ArgumentNullException("groupInfo");

			try
			{
				GroupSecurityHelper.DemandPermission(groupInfo);

				GroupInfo gi = GetUserGroupDAO().SaveGroup(groupInfo);
				return gi;
			}
			catch (AuthorizingException) { throw; }
			catch (GroupManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateGroupMException(DescriptionResource.GME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void DeleteGroup(Guid groupID)
		{
			try
			{
				GroupInfo groupInfo = GetGroupInfo(groupID);
				GroupSecurityHelper.DemandPermission(groupInfo);

				GetUserGroupDAO().RemoveGroup(groupID);
			}
			catch (AuthorizingException) { throw; }
			catch (GroupManipulationException) { throw; }
			catch (Exception exc)
			{
				throw CreateGroupMException(DescriptionResource.GME_SaveError, exc);
			}
		}

		private IUserGroupDAO GetUserGroupDAO()
		{
			return daoFactory.GetUserGroupDAO();
		}

		private static GroupCategoryManipulationException CreateGCMException(string message, Exception exc)
		{
			if (exc != null) log.Error("CreateGCMException", exc);
			return new GroupCategoryManipulationException(message, exc != null ? exc.ToString() : null);
		}

		private static GroupManipulationException CreateGroupMException(string message, Exception exc)
		{
			if (exc != null) log.Error("CreateGroupMException", exc);
			return new GroupManipulationException(message, exc != null ? exc.ToString() : null);
		}
	}
}
