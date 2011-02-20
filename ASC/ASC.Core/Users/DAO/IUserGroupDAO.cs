using System;
using System.Collections.Generic;

namespace ASC.Core.Users.DAO
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUserGroupDAO
	{
		#region Users

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		List<UserInfo> GetUsers();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		UserInfo GetUser(Guid userID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		UserInfo SaveUser(UserInfo user);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		void RemoveUser(Guid userID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="moduleID"></param>
		/// <param name="photo"></param>
		void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="moduleID"></param>
		/// <returns></returns>
		byte[] GetUserPhoto(Guid userID, Guid moduleID);

		#endregion


		#region Groups

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryID"></param>
		/// <returns></returns>
		List<GroupInfo> GetGroups();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupID"></param>
		/// <returns></returns>
		GroupInfo GetGroup(Guid groupID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		GroupInfo SaveGroup(GroupInfo group);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groupID"></param>
		void RemoveGroup(Guid groupID);

		#endregion


		#region User - Group References

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		List<UserGroupReference> GetUserGroupRefs();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="groupID"></param>
		/// <returns></returns>
		bool ExistsUserGroupRef(Guid userID, Guid groupID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="groupID"></param>
		void SaveUserGroupRef(Guid userID, Guid groupID);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userID"></param>
		/// <param name="groupID"></param>
		void RemoveUserGroupRef(Guid userID, Guid groupID);
		
		#endregion
	}
}