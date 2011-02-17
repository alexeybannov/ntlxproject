using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Core.Common;
using ASC.Core.Common.Cache;
using ASC.Core.Users;

namespace ASC.Core
{
    class ClientUserManager : IUserManagerClient, IGroupManagerClient
    {
        private readonly IDictionary<int, ICache<Guid, UserInfo>> usersCache;
        private readonly IDictionary<int, ICache<string, DepartmentManagerRef>> managersCache;
        private readonly IDictionary<int, ICache<string, UserGroupReference>> userGroupRefsCache;
        private readonly IDictionary<int, ICache<Guid, GroupInfo>> groupsCache;


        private ICache<Guid, UserInfo> UsersCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (usersCache)
                {
                    if (!usersCache.ContainsKey(tenant))
                    {
                        usersCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<Guid, UserInfo>(Constants.CacheIdUsers + tenant, SyncUsers);
                    }
                    return usersCache[tenant];
                }
            }
        }

        private ICache<string, DepartmentManagerRef> ManagersCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (managersCache)
                {
                    if (!managersCache.ContainsKey(tenant))
                    {
                        var managersCacheInfo = new CacheInfo(Constants.CacheIdMamagers + tenant);
                        managersCacheInfo.AddParentCache(Constants.CacheIdCategories + tenant);
                        managersCacheInfo.AddParentCache(Constants.CacheIdGroups + tenant);
                        managersCacheInfo.AddParentCache(Constants.CacheIdUsers + tenant);
                        managersCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<string, DepartmentManagerRef>(managersCacheInfo, SyncManagers);
                    }
                    return managersCache[tenant];
                }
            }
        }

        private ICache<string, UserGroupReference> UserGroupRefsCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (userGroupRefsCache)
                {
                    if (!userGroupRefsCache.ContainsKey(tenant))
                    {
                        var userGroupRefsCacheInfo = new CacheInfo(Constants.CacheIdGroupUserRef + tenant);
                        userGroupRefsCacheInfo.AddParentCache(Constants.CacheIdCategories + tenant);
                        userGroupRefsCacheInfo.AddParentCache(Constants.CacheIdGroups + tenant);
                        userGroupRefsCacheInfo.AddParentCache(Constants.CacheIdUsers + tenant);
                        userGroupRefsCache[tenant] =
                            CoreContext.CacheInfoStorage.CreateCache<string, UserGroupReference>(
                                userGroupRefsCacheInfo, SyncUserGroupRefs);
                    }
                    return userGroupRefsCache[tenant];
                }
            }
        }

        private ICache<Guid, GroupInfo> GroupsCache
        {
            get
            {
                int tenant = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                lock (groupsCache)
                {
                    if (!groupsCache.ContainsKey(tenant))
                    {
                        groupsCache[tenant] = CoreContext.CacheInfoStorage.CreateCache<Guid, GroupInfo>(Constants.CacheIdGroups + tenant, SyncGroups);
                    }
                    return groupsCache[tenant];
                }
            }
        }


        internal ClientUserManager()
        {
            usersCache = new Dictionary<int, ICache<Guid, UserInfo>>();
            managersCache = new Dictionary<int, ICache<string, DepartmentManagerRef>>();
            userGroupRefsCache = new Dictionary<int, ICache<string, UserGroupReference>>();
            groupsCache = new Dictionary<int, ICache<Guid, GroupInfo>>();
        }

        #region Implementation IUserManager

        public UserInfo[] GetUsers()
        {
            return GetUsers(EmployeeStatus.Default);
        }

        public UserInfo[] GetUsers(EmployeeStatus status)
        {
            return new List<UserInfo>(UsersCache.Values)
                .FindAll(u => (u.Status & status) == u.Status && IsUserInGroup(u.ID, Constants.GroupUser.ID))
                .ToArray();
        }

        public string[] GetUserNames(EmployeeStatus status)
        {
            var users = new List<UserInfo>(GetUsers(status));
            users.RemoveAll(u => string.IsNullOrEmpty(u.UserName));
            return users.ConvertAll(u => u.UserName).ToArray();
        }

        public UserInfo GetUserByUserName(string userName)
        {
            UserInfo user = new List<UserInfo>(UsersCache.Values)
                .Find(u => string.Compare(u.UserName, userName, true) == 0);
            return user ?? Constants.LostUser;
        }

        public bool IsUserNameExists(string userName)
        {
            return Array.Exists(
                GetUserNames(EmployeeStatus.All),
                s => string.Compare(userName, s, StringComparison.InvariantCultureIgnoreCase) == 0
                );
        }

        public UserInfo GetUsers(Guid userID)
        {
            if (UsersCache.ContainsKey(userID)) return UsersCache[userID];
            ISysAccount sys = Configuration.Constants.SystemAccounts.SingleOrDefault(a => a.ID == userID);
            return sys != null
                       ? new UserInfo { ID = sys.ID, LastName = sys.Name }
                       : Constants.LostUser;
        }

        public bool UserExists(Guid userID)
        {
            return UsersCache.ContainsKey(userID) || IsSystemUser(userID);
        }

        public UserInfo GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return Constants.LostUser;
            UserInfo user = new List<UserInfo>(UsersCache.Values)
                .Find(u => string.Compare(u.Email, email, true) == 0);
            return user ?? Constants.LostUser;
        }

        public UserInfo[] Search(string text)
        {
            return Search(text, EmployeeStatus.Default);
        }

        public UserInfo[] Search(string text, UserSearchType searchType)
        {
            return Search(text, EmployeeStatus.Default, searchType);
        }

        public UserInfo[] Search(string text, EmployeeStatus status)
        {
            return Search(text, status, Guid.Empty);
        }

        public UserInfo[] Search(string text, EmployeeStatus status, UserSearchType searchType)
        {
            return Search(text, status, Guid.Empty, searchType);
        }

        public UserInfo[] Search(string text, EmployeeStatus status, Guid groupId)
        {
            return Search(text, status, groupId, UserSearchType.StartWith);
        }

        public UserInfo[] Search(string text, EmployeeStatus status, Guid groupId, UserSearchType searchType)
        {
            if (text == null || text.Trim() == string.Empty) return new UserInfo[0];
            string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return new UserInfo[0];
            List<UserInfo> users = null;
            if (groupId == Guid.Empty)
            {
                users = new List<UserInfo>(GetUsers(status));
            }
            else
            {
                users = new List<UserInfo>(GetUsersByGroup(groupId, IncludeType.Distinct | IncludeType.InChild));
                users = users.FindAll(u => (u.Status & status) == status);
            }
            var findUsers = new List<UserInfo>();
            var properties = new string[3];
            foreach (UserInfo user in users)
            {
                properties[0] = user.LastName ?? string.Empty;
                properties[1] = user.FirstName ?? string.Empty;
                properties[2] = user.Title ?? string.Empty;
                if (IsPropertiesContainsWords(properties, words, searchType))
                {
                    findUsers.Add(user);
                }
            }
            return findUsers.ToArray();
        }

        private bool IsPropertiesContainsWords(IEnumerable<string> properties, IEnumerable<string> words, UserSearchType searchType)
        {
            foreach (string word in words)
            {
                bool find = false;
                foreach (string property in properties)
                {
                    switch (searchType)
                    {
                        case UserSearchType.StartWith:
                            find = property.ToUpper().StartsWith(word.ToUpper());
                            break;
                        case UserSearchType.Contains:
                            find = property.ToUpper().Contains(word.ToUpper());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(string.Format("Unknown UserSearchType: {0}.",
                                                                                searchType));
                    }
                    if (find) break;
                }
                if (!find) return false;
            }
            return true;
        }

        public UserInfo SaveUserInfo(UserInfo userInfo)
        {
            if (IsSystemUser(userInfo.ID)) return GetUsers(userInfo.ID);
            if (ReferenceEquals(userInfo, Constants.LostUser) || Equals(userInfo, Constants.LostUser))
                return Constants.LostUser;
            try
            {
                UserInfo user = CoreContext.InternalUserManager.SaveUserInfo(userInfo);
                UsersCache[user.ID] = user;
                return user;
            }
            catch
            {
                UsersCache.Synchronize();
                throw;
            }
        }

        public void DeleteUser(Guid userID)
        {
            if (Constants.LostUser.ID == userID || IsSystemUser(userID)) return;
            CoreContext.InternalUserManager.DeleteUser(userID);
            UsersCache.Remove(userID);
        }

        public void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo)
        {
            if (Constants.LostUser.ID == userID || IsSystemUser(userID)) return;
            CoreContext.InternalUserManager.SaveUserPhoto(userID, Guid.Empty, photo);
        }

        public byte[] GetUserPhoto(Guid userID, Guid moduleID)
        {
            if (Constants.LostUser.ID == userID || IsSystemUser(userID)) return null;
            byte[] photo = CoreContext.InternalUserManager.GetUserPhoto(userID, Guid.Empty);
            return photo != null && photo.Length == 0 ? null : photo;
        }

        public GroupInfo[] GetUserGroups(Guid userID)
        {
            return GetUserGroups(userID, Guid.Empty);
        }

        public GroupInfo[] GetUserGroups(Guid userID, Guid categoryID)
        {
            var groups = new List<GroupInfo>(GetUserGroups(userID, IncludeType.Distinct));
            return groups
                .FindAll(g => g.CategoryID == categoryID)
                .ToArray();
        }

        public GroupInfo[] GetUserGroups(Guid userID, IncludeType includeType)
        {
            var result = new List<GroupInfo>();
            var distinctUserGroups = new List<GroupInfo>();
            foreach (var g in CoreContext.GroupManager.GetGroups())
            {
                if (IsUserInGroup(userID, g.ID)) distinctUserGroups.Add(g);
            }
            foreach (var g in Constants.BuildinGroups)
            {
                if (IsUserInGroup(userID, g.ID)) distinctUserGroups.Add(g);
            }
            if (IncludeType.Distinct == (includeType & IncludeType.Distinct))
            {
                result.AddRange(distinctUserGroups);
            }
            if (IncludeType.InParent == (includeType & IncludeType.InParent))
            {
                foreach (GroupInfo group in distinctUserGroups)
                {
                    GroupInfo current = group.Parent;
                    while (current != null)
                    {
                        if (!result.Contains(current)) result.Add(current);
                        current = current.Parent;
                    }
                }
            }
            if (IncludeType.InChild == (includeType & IncludeType.InChild))
            {
                distinctUserGroups.ForEach(g => RecursiveAddChildGroups(g, distinctUserGroups));
            }
            return result.ToArray();
        }

        public bool IsUserInGroup(Guid userID, Guid groupID)
        {
            if (groupID == Constants.GroupEveryone.ID) return true;

            if (groupID == Constants.GroupUser.ID || groupID == Constants.GroupVisitor.ID)
            {
                var visitorUgr = new UserGroupReference(userID, Constants.GroupVisitor.ID);
                if (groupID == Constants.GroupUser.ID) return !UserGroupRefsCache.ContainsKey(visitorUgr.Id);
                return UserGroupRefsCache.ContainsKey(visitorUgr.Id);
            }
            var ugr = new UserGroupReference(userID, groupID);
            return UserGroupRefsCache.ContainsKey(ugr.Id);
        }

        public UserInfo[] GetUsersByGroup(Guid groupID, IncludeType includeType)
        {
            GroupInfo group = CoreContext.GroupManager.GetGroupInfo(groupID);
            if (Constants.LostGroupInfo.Equals(group)) return new UserInfo[0];
            var result = new List<UserInfo>();
            foreach (UserInfo u in GetUsers())
            {
                if (IsUserInGroup(u.ID, groupID)) result.Add(u);
            }
            if (IncludeType.InParent == (includeType & IncludeType.InParent))
            {
                GroupInfo current = group.Parent;
                while (current != null)
                {
                    foreach (UserInfo u in GetUsersByGroup(group.ID, IncludeType.Distinct))
                    {
                        if (!result.Contains(u)) result.Add(u);
                    }
                    current = current.Parent;
                }
            }
            if (IncludeType.InChild == (includeType & IncludeType.InChild))
            {
                RecursiveFindUsersInGroup(group.Descendants, result);
            }
            return result.ToArray();
        }

        public void AddUserIntoGroup(Guid userID, Guid groupID)
        {
            if (Constants.LostUser.ID == userID ||
                Constants.LostGroupInfo.ID == groupID ||
                IsUserInGroup(userID, groupID)) return;
            if (Array.Exists(Constants.BuildinGroups, g => g.ID == groupID) && groupID != Constants.GroupAdmin.ID)
            {
                throw new UserManipulationException(CommonDescriptionResource.UserManipulationException_BuildInEdit);
            }
            try
            {
                CoreContext.InternalUserManager.AddUserIntoGroup(userID, groupID);
            }
            finally
            {
                UserGroupRefsCache.Flush();
            }
        }

        public void RemoveUserFromGroup(Guid userID, Guid groupID)
        {
            if (Constants.LostUser.ID == userID ||
                Constants.LostGroupInfo.ID == groupID ||
                !IsUserInGroup(userID, groupID)) return;
            if (Array.Exists(Constants.BuildinGroups, g => g.ID == groupID) && groupID != Constants.GroupAdmin.ID)
            {
                throw new UserManipulationException(CommonDescriptionResource.UserManipulationException_BuildInEdit);
            }
            try
            {
                CoreContext.InternalUserManager.RemoveUserFromGroup(userID, groupID);
            }
            finally
            {
                UserGroupRefsCache.Flush();
            }
        }

        public UserInfo GetCompanyCEO()
        {
            var id = GetDepartmentManager(Guid.Empty);
            return id != Guid.Empty ? GetUsers(id) : null;
        }

        public void SetCompanyCEO(Guid userId)
        {
            RemoveDepartmentManager(Guid.Empty, GetDepartmentManager(Guid.Empty));
            SetDepartmentManager(Guid.Empty, userId);
        }

        public GroupInfo[] GetDepartments()
        {
            return CoreContext.GroupManager.GetGroups();
        }

        public Guid GetDepartmentManager(Guid deparmentID)
        {
            foreach (var r in ManagersCache.Values)
            {
                if (r.DepartmentId == deparmentID) return r.ManagerId;
            }
            return Guid.Empty;
        }

        public void SetDepartmentManager(Guid deparmentID, Guid userID)
        {
            var managerId = GetDepartmentManager(deparmentID);
            if (managerId != Guid.Empty)
            {
                RemoveDepartmentManager(deparmentID, managerId);
            }
            CoreContext.InternalUserManager.SetDepartmentManager(deparmentID, userID);
            var r = new DepartmentManagerRef(deparmentID, userID);
            ManagersCache[r.Id] = r;
        }

        public bool RemoveDepartmentManager(Guid deparmentID, Guid userID)
        {
            var result = CoreContext.InternalUserManager.RemoveDepartmentManager(deparmentID, userID);
            var r = new DepartmentManagerRef(deparmentID, userID);
            ManagersCache.Remove(r.Id);
            return result;
        }

        public bool IsDepartmentManager(Guid deparmentID, Guid userID)
        {
            if (IsUserCEO(userID)) return true;
            var r = new DepartmentManagerRef(deparmentID, userID);
            return ManagersCache.ContainsKey(r.Id);
        }

        public bool IsManager(Guid userID)
        {
            if (IsUserCEO(userID)) return true;
            foreach (var r in ManagersCache.Values)
            {
                if (r.ManagerId == userID) return true;
            }
            return false;
        }

        public bool IsManager(Guid managerID, Guid userID)
        {
            if (IsUserCEO(managerID)) return true;
            if (IsUserCEO(userID)) return false;
            if (!IsManager(managerID)) return false;

            foreach (var g in GetUserGroups(userID, IncludeType.InParent | IncludeType.Distinct))
            {
                if (IsDepartmentManager(g.ID, managerID)) return true;
            }
            return false;
        }

        #endregion


        private bool IsUserCEO(Guid userId)
        {
            var ceo = GetCompanyCEO();
            return ceo != null && ceo.ID == userId;
        }

        private IDictionary<Guid, UserInfo> SyncUsers()
        {
            return CoreContext.InternalUserManager.GetUsers().ToDictionary(u => u.ID);
        }

        private IDictionary<string, DepartmentManagerRef> SyncManagers()
        {
            return CoreContext.InternalUserManager.GetDepartmentManagerRefs().ToDictionary(r => r.Id);
        }

        private IDictionary<string, UserGroupReference> SyncUserGroupRefs()
        {
            return CoreContext.InternalUserManager.GetUsersGroupReferences().ToDictionary(r => r.Id);
        }

        private void RecursiveAddChildGroups(GroupInfo parent, List<GroupInfo> result)
        {
            if (parent == null || parent.Descendants == null) return;
            foreach (GroupInfo group in parent.Descendants)
            {
                if (!result.Contains(group)) result.Add(group);
                RecursiveAddChildGroups(group, result);
            }
        }

        private void RecursiveFindUsersInGroup(IEnumerable<GroupInfo> parents, List<UserInfo> result)
        {
            if (parents == null) return;
            foreach (GroupInfo group in parents)
            {
                foreach (UserInfo u in GetUsersByGroup(group.ID, IncludeType.Distinct))
                {
                    if (!result.Contains(u)) result.Add(u);
                }
                RecursiveFindUsersInGroup(group.Descendants, result);
            }
        }

        private bool IsSystemUser(Guid userID)
        {
            return Array.Exists(Configuration.Constants.SystemAccounts, a => a.ID == userID);
        }


        #region IGroupManager

        public GroupInfo[] GetGroups()
        {
            return GetGroups(Guid.Empty);
        }

        public GroupInfo[] GetGroups(Guid categoryID)
        {
            return new List<GroupInfo>(GroupsCache.Values)
                .FindAll(g => g.Parent == null && g.CategoryID == categoryID)
                .ToArray();
        }

        public GroupInfo GetGroupInfo(Guid groupID)
        {
            return FindGroupInfo(GroupsCache.Values, groupID) ?? Constants.LostGroupInfo;
        }

        public GroupInfo SaveGroupInfo(GroupInfo groupInfo)
        {
            if (ReferenceEquals(groupInfo, Constants.LostGroupInfo) || Equals(groupInfo, Constants.LostGroupInfo))
            {
                return Constants.LostGroupInfo;
            }
            if (Array.Find(Constants.BuildinGroups, gr => gr.ID == groupInfo.ID) != null)
            {
                throw new UserManipulationException(CommonDescriptionResource.UserManipulationException_BuildInEdit);
            }
            groupInfo = CoreContext.InternalGroupManager.SaveGroupInfo(groupInfo);
            GroupsCache.Flush();
            return groupInfo;
        }

        public void DeleteGroup(Guid groupID)
        {
            if (groupID == Constants.LostGroupInfo.ID) return;
            if (Array.Find(Constants.BuildinGroups, gr => gr.ID == groupID) != null)
                throw new UserManipulationException(CommonDescriptionResource.UserManipulationException_BuildInEdit);
            CoreContext.InternalGroupManager.DeleteGroup(groupID);
            GroupsCache.Flush();
            UsersCache.Flush();
        }

        private GroupInfo FindGroupInfo(ICollection<GroupInfo> groups, Guid groupID)
        {
            if (groups == null) return null;
            foreach (GroupInfo g in groups)
            {
                if (g.ID == groupID) return g;
                var findedGroup = FindGroupInfo(g.Descendants, groupID);
                if (findedGroup != null) return findedGroup;
            }
            return null;
        }

        private IDictionary<Guid, GroupInfo> SyncGroups()
        {
            return CoreContext.InternalGroupManager.GetGroups()
                .Union(Constants.BuildinGroups)
                .ToDictionary(g => g.ID);
        }

        #endregion IGroupManager
    }
}