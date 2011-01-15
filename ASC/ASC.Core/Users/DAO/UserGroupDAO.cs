using System;
using System.Collections.Generic;
using ASC.Common.Data;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Data.Sql;

namespace ASC.Core.Users.DAO
{
	class UserGroupDAO : DAOBase, IUserGroupDAO
	{
		public UserGroupDAO(string dbId, int tenant)
			: base(dbId, tenant)
		{

		}

		#region IUserGroupDAO Members

		public List<UserInfo> GetUsers()
		{
			return DbManager
				.ExecuteList(Query("core_user").Select(UserGroupMapper.UserColumns))
				.MapToObject<UserInfo>(UserGroupMapper.ToUser);
		}

		public UserInfo GetUser(Guid userID)
		{
			var users = DbManager
				.ExecuteList(Query("core_user").Select(UserGroupMapper.UserColumns).Where("ID", userID.ToString()))
				.MapToObject<UserInfo>(UserGroupMapper.ToUser);

			return 0 < users.Count ? users[0] : null;
		}

		public UserInfo SaveUser(UserInfo u)
		{
			using (var tx = DbManager.BeginTransaction())
			{
				var status = (EmployeeStatus)0;
				if (u.ID == Guid.Empty) u.ID = Guid.NewGuid();
				else status = (EmployeeStatus)DbManager.ExecuteScalar<long>(Query("core_user").Select("Status").Where("ID", u.ID.ToString()));

				if (status != u.Status && u.Status == EmployeeStatus.Terminated)
				{
					u.TerminatedDate = DateTime.Now;
					RemoveForeign(u.ID.ToString());
				}
				var timestamp = DateTime.UtcNow;

				DbManager.ExecuteNonQuery(
					Insert("core_user")
					.InColumns(UserGroupMapper.UserColumns)
					.Values(u.ID.ToString(), u.FirstName, u.LastName, u.Sex, u.BirthDate, u.UserName, u.Status,
						u.Title, u.Department, u.WorkFromDate, u.TerminatedDate,
						u.ContactsToString(), u.Email,
						u.Location, u.Notes, DateTime.UtcNow)
				);

				tx.Commit();
			}
			return u;
		}

		public void RemoveUser(Guid userID)
		{
			using (var tx = DbManager.BeginTransaction())
			{
				RemoveForeign(userID.ToString());
				DbManager.ExecuteNonQuery(Delete("core_groupmanager").Where("UserID", userID.ToString()));
				DbManager.ExecuteNonQuery(Delete("core_usergroup").Where("UserID", userID.ToString()));
				DbManager.ExecuteNonQuery(Delete("core_userphoto").Where("UserID", userID.ToString()));
				DbManager.ExecuteNonQuery(Delete("core_usersecurity").Where("UserID", userID.ToString()));
				DbManager.ExecuteNonQuery(Delete("core_user").Where("ID", userID.ToString()));

				tx.Commit();
			}
		}

		public void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo)
		{
			if (photo == null || photo.Length == 0)
			{
				DbManager.ExecuteNonQuery(
					Delete("core_userphoto").Where("UserID", userID.ToString()).Where("ModuleID", moduleID.ToString())
				);
			}
			else
			{
				DbManager.ExecuteNonQuery(
					Insert("core_userphoto").InColumns("UserID", "ModuleID", "Photo").Values(userID.ToString(), moduleID.ToString(), photo)
				);
			}
		}

		public byte[] GetUserPhoto(Guid userID, Guid moduleID)
		{
			return DbManager.ExecuteScalar<byte[]>(
				Query("core_userphoto").Select("Photo").Where("UserID", userID.ToString()).Where("ModuleID", moduleID.ToString())
			);
		}


		public List<GroupCategory> GetCategories()
		{
			return DbManager
				.ExecuteList(Query("core_groupcategory").Select(UserGroupMapper.CategoryColumns))
				.MapToObject<GroupCategory>(UserGroupMapper.ToCategory);
		}

		public GroupCategory GetCategory(Guid categoryID)
		{
			var categories = DbManager
				.ExecuteList(Query("core_groupcategory").Select(UserGroupMapper.CategoryColumns).Where("ID", categoryID.ToString()))
				.MapToObject<GroupCategory>(UserGroupMapper.ToCategory);

			return 0 < categories.Count ? categories[0] : null;
		}

		public GroupCategory SaveCategory(GroupCategory c)
		{
			using (var tx = DbManager.BeginTransaction())
			{
				if (c.ID == Guid.Empty) c.ID = Guid.NewGuid();
				if (c.ModuleID == Guid.Empty)
				{
					var oldGuid = DbManager.ExecuteScalar<string>(Query("core_groupcategory").Select("ID").Where("ModuleID", Guid.Empty.ToString()));
					if (!string.IsNullOrEmpty(oldGuid) && new Guid(oldGuid) != c.ID) throw new OnlyOneDefaultCategoryException();
				}

				DbManager.ExecuteNonQuery(
					Insert("core_groupcategory")
					.InColumns(UserGroupMapper.CategoryColumns)
					.Values(c.ID.ToString(), c.ModuleID.ToString(), c.Name, c.Description, c.GroupType)
				);

				tx.Commit();
			}
			return c;
		}

		public void RemoveCategory(Guid categoryID)
		{
			using (var tx = DbManager.BeginTransaction())
			{
				var groupIds = DbManager
					.ExecuteList(Query("core_group").Select("ID").Where("CategoryID", categoryID.ToString()))
					.ConvertAll<string>(r => Convert.ToString(r[0]));

				foreach (var groupId in groupIds)
				{
					RemoveGroupInternal(groupId);
				}
				DbManager.ExecuteNonQuery(Delete("core_groupcategory").Where("ID", categoryID.ToString()));

				tx.Commit();
			}
		}

		public List<GroupInfo> GetGroups()
		{
			return GetAllGroups().FindAll(g => g.Parent == null);
		}

		public GroupInfo GetGroup(Guid groupID)
		{
			var rawData = DbManager
				.ExecuteList(
					Query("core_group")
					.Select(UserGroupMapper.GroupColumns)
					.Select(Query("core_group g").SelectCount().Where("g.ParentID", groupID.ToString()))
					.Where("ID", groupID.ToString()));

			if (rawData.Count == 0) return null;

			if (rawData[0][4] != null || 0 < Convert.ToInt64(rawData[0][5]))
			{
				return GetAllGroups().Find(g => g.ID == groupID);
			}
			var group = UserGroupMapper.ToGroup(rawData[0]);
			return group;
		}

		private List<GroupInfo> GetAllGroups()
		{
			var groups = new Dictionary<Guid, GroupInfo>();
			var query = Query("core_group").Select(UserGroupMapper.GroupColumns);

			return DbManager
				.ExecuteList(query)
				.MapToObject<GroupInfo>(UserGroupMapper.ToGroup)
				.ForEach<GroupInfo>(g =>
				{
					groups[g.ID] = g;
				})
				.ForEach<GroupInfo>(g =>
				{
					if (g.ParentID != Guid.Empty && groups.ContainsKey(g.ParentID)) groups[g.ParentID].AddDescendant(g);
				});
		}

		public GroupInfo SaveGroup(GroupInfo g)
		{
			if (g.ID == Guid.Empty) g.ID = Guid.NewGuid();
			DbManager.ExecuteNonQuery(
				Insert("core_group")
				.InColumns(UserGroupMapper.GroupColumns)
				.Values(g.ID.ToString(), g.Name, g.Description, g.CategoryID.ToString(), g.Parent != null ? g.Parent.ID.ToString() : null)
			);

			return g;
		}

		public void RemoveGroup(Guid groupID)
		{
			using (var tx = DbManager.BeginTransaction())
			{
				RemoveGroupInternal(groupID.ToString());
				tx.Commit();
			}
		}

		public List<UserGroupReference> GetUserGroupRefs()
		{
			return DbManager
				.ExecuteList(Query("core_usergroup").Select(UserGroupMapper.UserGroupColumns))
				.MapToObject<UserGroupReference>(UserGroupMapper.ToUserGroupRef);
		}

		public bool ExistsUserGroupRef(Guid userID, Guid groupID)
		{
			return 0 < DbManager.ExecuteScalar<long>(Query("core_usergroup").SelectCount().Where("UserID", userID.ToString()).Where("GroupID", groupID.ToString()));
		}

		public void SaveUserGroupRef(Guid userID, Guid groupID)
		{
			DbManager.ExecuteNonQuery(
				Insert("core_usergroup").InColumns(UserGroupMapper.UserGroupColumns).Values(userID.ToString(), groupID.ToString())
			);
		}

		public void RemoveUserGroupRef(Guid userID, Guid groupID)
		{
			DbManager.ExecuteNonQuery(
				Delete("core_usergroup").Where("UserID", userID.ToString()).Where("GroupID", groupID.ToString())
			);
		}

		#endregion

		private void RemoveGroupInternal(string groupID)
		{
			var groupIds = DbManager
				.ExecuteList(Query("core_group").Select("ID").Where("ParentID", groupID))
				.ConvertAll<string>(r => Convert.ToString(r[0]));

			foreach (var id in groupIds)
			{
				RemoveGroupInternal(id);
			}

			RemoveForeign(groupID);
			DbManager.ExecuteNonQuery(Delete("core_groupmanager").Where("GroupID", groupID));
			DbManager.ExecuteNonQuery(Delete("core_group").Where("ID", groupID));
            DbManager.ExecuteNonQuery(Update("core_user").Set("Department", null).Where(Exp.In("id", new SqlQuery("core_usergroup").Select("userid").Where("groupid", groupID))));
			DbManager.ExecuteNonQuery(Delete("core_usergroup").Where("GroupID", groupID));
		}

		private void RemoveForeign(string id)
		{
			DbManager.ExecuteNonQuery(Delete("core_acl").Where("Subject", id));
			DbManager.ExecuteNonQuery(Delete("core_subscription").Where("Recipient", id));
			DbManager.ExecuteNonQuery(Delete("core_subscriptionmethod").Where("Recipient", id));
		}
	}
}