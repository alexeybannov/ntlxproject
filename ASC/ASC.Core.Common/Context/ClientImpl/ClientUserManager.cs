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
        private IUserService userService;

        private readonly IDictionary<Guid, UserInfo> systemUsers;


        public ClientUserManager(IUserService userService)
        {
            this.userService = userService;

            systemUsers = Configuration.Constants.SystemAccounts.ToDictionary(a => a.ID, a => new UserInfo { ID = a.ID, LastName = a.Name });
            systemUsers[Constants.LostUser.ID] = Constants.LostUser;
        }


        #region Implementation IUserManager

        public UserInfo[] GetUsers()
        {
            return GetUsers(EmployeeStatus.Default);
        }

        public UserInfo[] GetUsers(EmployeeStatus status)
        {
            return GetUsersInternal()
                .Where(u => (u.Status & status) == u.Status && IsUserInGroup(u.ID, Constants.GroupUser.ID))
                .ToArray();
        }

        public string[] GetUserNames(EmployeeStatus status)
        {
            return GetUsers(status)
                .Select(u => u.UserName)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        public UserInfo GetUserByUserName(string username)
        {
            return GetUsersInternal()
                .SingleOrDefault(u => string.Compare(u.UserName, username, StringComparison.CurrentCultureIgnoreCase) == 0) ?? Constants.LostUser;
        }

        public bool IsUserNameExists(string username)
        {
            return GetUserNames(EmployeeStatus.All).Contains(username, StringComparer.CurrentCultureIgnoreCase);
        }

        public UserInfo GetUsers(Guid id)
        {
            if (systemUsers.ContainsKey(id)) return systemUsers[id];
            return GetUsersInternal().SingleOrDefault(u => u.ID == id) ?? Constants.LostUser;
        }

        public bool UserExists(Guid id)
        {
            return !GetUsers(id).Equals(Constants.LostUser);
        }

        public UserInfo GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return Constants.LostUser;

            return GetUsersInternal()
                .SingleOrDefault(u => string.Compare(u.Email, email, StringComparison.CurrentCultureIgnoreCase) == 0) ?? Constants.LostUser;
        }

        public UserInfo[] Search(string text, EmployeeStatus status)
        {
            return Search(text, status, Guid.Empty);
        }

        public UserInfo[] Search(string text, EmployeeStatus status, Guid groupId)
        {
            if (text == null || text.Trim() == string.Empty) return new UserInfo[0];

            var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return new UserInfo[0];

            var users = groupId == Guid.Empty ?
                GetUsers(status) :
                GetUsersByGroup(groupId).Where(u => (u.Status & status) == status);

            var findUsers = new List<UserInfo>();
            var properties = new string[3];
            foreach (var user in users)
            {
                properties[0] = user.LastName ?? string.Empty;
                properties[1] = user.FirstName ?? string.Empty;
                properties[2] = user.Title ?? string.Empty;
                if (IsPropertiesContainsWords(properties, words))
                {
                    findUsers.Add(user);
                }
            }
            return findUsers.ToArray();
        }

        public UserInfo SaveUserInfo(UserInfo u)
        {
            if (systemUsers.ContainsKey(u.ID)) return systemUsers[u.ID];
            return ToUserInfo(userService.SaveUser(CoreContext.TenantManager.GetCurrentTenant().TenantId, ToUser(u)));
        }

        public void DeleteUser(Guid id)
        {
            if (systemUsers.ContainsKey(id)) return;
            userService.RemoveUser(CoreContext.TenantManager.GetCurrentTenant().TenantId, id);
        }

        public void SaveUserPhoto(Guid id, Guid notused, byte[] photo)
        {
            if (systemUsers.ContainsKey(id)) return;
            userService.SetUserPhoto(CoreContext.TenantManager.GetCurrentTenant().TenantId, id, photo);
        }

        public byte[] GetUserPhoto(Guid id, Guid notused)
        {
            if (systemUsers.ContainsKey(id)) return null;
            return userService.GetUserPhoto(CoreContext.TenantManager.GetCurrentTenant().TenantId, id);
        }

        public GroupInfo[] GetUserGroups(Guid id)
        {
            return GetUserGroups(id, Guid.Empty);
        }

        public GroupInfo[] GetUserGroups(Guid id, Guid categoryID)
        {
            return GetUserGroups(id, IncludeType.Distinct)
                .Where(g => g.CategoryID == categoryID)
                .ToArray();
        }

        public GroupInfo[] GetUserGroups(Guid userID, IncludeType includeType)
        {
            var result = new List<GroupInfo>();
            var distinctUserGroups = new List<GroupInfo>();
            foreach (var g in GetGroups())
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
                foreach (var group in distinctUserGroups)
                {
                    var current = group.Parent;
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

        public bool IsUserInGroup(Guid userId, Guid groupId)
        {
            if (groupId == Constants.GroupEveryone.ID) return true;

            var refs = GetRefsInternal();
            if (groupId == Constants.GroupUser.ID || groupId == Constants.GroupVisitor.ID)
            {
                var visitorRef = refs.SingleOrDefault(r => r.RefType == UserGroupRefType.Contains && r.UserId == userId && r.GroupId == Constants.GroupVisitor.ID);
                if (groupId == Constants.GroupUser.ID) return visitorRef != null;
                return visitorRef == null;
            }
            return refs.SingleOrDefault(r => r.RefType == UserGroupRefType.Contains && r.UserId == userId && r.GroupId == groupId) != null;
        }

        public UserInfo[] GetUsersByGroup(Guid groupId)
        {
            var group = GetGroupInfo(groupId);
            if (Constants.LostGroupInfo.Equals(group)) return new UserInfo[0];

            return GetUsers().Where(u => IsUserInGroup(u.ID, groupId)).ToArray();
        }

        public void AddUserIntoGroup(Guid userId, Guid groupId)
        {
            if (Constants.LostUser.ID == userId || Constants.LostGroupInfo.ID == groupId) return;

            userService.SaveUserGroupRef(
                CoreContext.TenantManager.GetCurrentTenant().TenantId,
                new UserGroupRef() { UserId = userId, GroupId = groupId, RefType = UserGroupRefType.Contains });
        }

        public void RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            if (Constants.LostUser.ID == userId || Constants.LostGroupInfo.ID == groupId) return;

            userService.RemoveUserGroupRef(CoreContext.TenantManager.GetCurrentTenant().TenantId, userId, groupId, UserGroupRefType.Contains);
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

        #endregion


        private void RecursiveAddChildGroups(GroupInfo parent, List<GroupInfo> result)
        {
            if (parent == null || parent.Descendants == null) return;
            foreach (var group in parent.Descendants)
            {
                if (!result.Contains(group)) result.Add(group);
                RecursiveAddChildGroups(group, result);
            }
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


        private bool IsPropertiesContainsWords(IEnumerable<string> properties, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var find = false;
                foreach (var p in properties)
                {
                    find = p.StartsWith(word, StringComparison.CurrentCultureIgnoreCase);
                    if (find) break;
                }
                if (!find) return false;
            }
            return true;
        }


        private IEnumerable<UserInfo> GetUsersInternal()
        {
            return userService.GetUsers(CoreContext.TenantManager.GetCurrentTenant().TenantId, default(DateTime))
                .Select(u => ToUserInfo(u));
        }

        private IEnumerable<UserGroupRef> GetRefsInternal()
        {
            return userService.GetUserGroupRefs(CoreContext.TenantManager.GetCurrentTenant().TenantId, default(DateTime));
        }

        private UserInfo ToUserInfo(User u)
        {
            return null;
        }

        private User ToUser(UserInfo u)
        {
            return null;
        }

        #endregion IGroupManager
    }
}