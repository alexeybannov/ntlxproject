using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Core.Users;

namespace ASC.Core
{
    class ClientUserManager : IUserManagerClient, IGroupManagerClient
    {
        private readonly IUserService userService;

        private readonly IDictionary<Guid, UserInfo> systemUsers;


        public ClientUserManager(IUserService userService)
        {
            this.userService = userService;

            systemUsers = Configuration.Constants.SystemAccounts.ToDictionary(a => a.ID, a => new UserInfo { ID = a.ID, LastName = a.Name });
            systemUsers[Constants.LostUser.ID] = Constants.LostUser;
        }


        #region Users

        public UserInfo[] GetUsers()
        {
            return GetUsers(EmployeeStatus.Default);
        }

        public UserInfo[] GetUsers(EmployeeStatus status)
        {
            return GetUsersInternal()
                .Where(u => (u.Status & status) == u.Status)
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
            return ToUserInfo(userService.GetUser(CoreContext.TenantManager.GetCurrentTenant().TenantId, id)) ?? Constants.LostUser;
        }

        public UserInfo GetUsers(int tenant, string login, string password)
        {
            return ToUserInfo(userService.GetUser(tenant, login, password)) ?? Constants.LostUser;
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
            if (u.ID == Guid.Empty) SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);
            else SecurityContext.DemandPermissions(new SecurityObjectId<UserInfo>(u.ID), new UserSecurityProvider(), Constants.Action_EditUser);

            var newUser = userService.SaveUser(CoreContext.TenantManager.GetCurrentTenant().TenantId, ToUser(u));
            return GetUsers(newUser.Id);
        }

        public void DeleteUser(Guid id)
        {
            if (systemUsers.ContainsKey(id)) return;
            SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);

            userService.RemoveUser(CoreContext.TenantManager.GetCurrentTenant().TenantId, id);
        }

        public void SaveUserPhoto(Guid id, Guid notused, byte[] photo)
        {
            if (systemUsers.ContainsKey(id)) return;
            SecurityContext.DemandPermissions(new SecurityObjectId<UserInfo>(id), new UserSecurityProvider(), Constants.Action_EditUser);

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
                var visitor = refs.Any(r => r.RefType == UserGroupRefType.Contains && r.UserId == userId && r.GroupId == Constants.GroupVisitor.ID);
                if (groupId == Constants.GroupUser.ID) return visitor;
                return !visitor;
            }
            return refs.Any(r => r.RefType == UserGroupRefType.Contains && r.UserId == userId && r.GroupId == groupId);
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
            SecurityContext.DemandPermissions(new[] { Constants.Action_EditGroups, Constants.Action_EditAz });

            userService.SaveUserGroupRef(
                CoreContext.TenantManager.GetCurrentTenant().TenantId,
                new UserGroupRef(userId, groupId, UserGroupRefType.Contains));
        }

        public void RemoveUserFromGroup(Guid userId, Guid groupId)
        {
            if (Constants.LostUser.ID == userId || Constants.LostGroupInfo.ID == groupId) return;
            SecurityContext.DemandPermissions(new[] { Constants.Action_EditGroups, Constants.Action_EditAz });

            userService.RemoveUserGroupRef(CoreContext.TenantManager.GetCurrentTenant().TenantId, userId, groupId, UserGroupRefType.Contains);
        }

        #endregion Users


        #region Company

        public GroupInfo[] GetDepartments()
        {
            return CoreContext.GroupManager.GetGroups();
        }

        public Guid GetDepartmentManager(Guid deparmentID)
        {
            return GetRefsInternal()
                .Where(r => r.RefType == UserGroupRefType.Manager && r.GroupId == deparmentID)
                .Select(r => r.UserId)
                .SingleOrDefault();
        }

        public void SetDepartmentManager(Guid deparmentID, Guid userID)
        {
            var managerId = GetDepartmentManager(deparmentID);
            if (managerId != Guid.Empty)
            {
                userService.RemoveUserGroupRef(
                    CoreContext.TenantManager.GetCurrentTenant().TenantId,
                    managerId, deparmentID, UserGroupRefType.Manager);
            }
            if (userID != Guid.Empty)
            {
                userService.SaveUserGroupRef(
                    CoreContext.TenantManager.GetCurrentTenant().TenantId,
                    new UserGroupRef(userID, deparmentID, UserGroupRefType.Manager));
            }
        }

        public UserInfo GetCompanyCEO()
        {
            var id = GetDepartmentManager(Guid.Empty);
            return id != Guid.Empty ? GetUsers(id) : null;
        }

        public void SetCompanyCEO(Guid userId)
        {
            SetDepartmentManager(Guid.Empty, userId);
        }

        #endregion Company


        #region Groups

        public GroupInfo[] GetGroups()
        {
            return GetGroups(Guid.Empty);
        }

        public GroupInfo[] GetGroups(Guid categoryID)
        {
            return GetGroupsInternal()
                .Where(g => g.Parent == null && g.CategoryID == categoryID)
                .ToArray();
        }

        public GroupInfo GetGroupInfo(Guid groupID)
        {
            return GetGroupsInternal()
                .SingleOrDefault(g => g.ID == groupID) ?? Constants.LostGroupInfo;
        }

        public GroupInfo SaveGroupInfo(GroupInfo g)
        {
            if (Constants.LostGroupInfo.Equals(g)) return Constants.LostGroupInfo;
            if (Constants.BuildinGroups.Any(b => b.ID == g.ID)) return Constants.BuildinGroups.Single(b => b.ID == g.ID);
            SecurityContext.DemandPermissions(new[] { Constants.Action_EditGroups, Constants.Action_EditAz });

            var newGroup = userService.SaveGroup(CoreContext.TenantManager.GetCurrentTenant().TenantId, ToGroup(g));
            return GetGroupInfo(newGroup.Id);
        }

        public void DeleteGroup(Guid id)
        {
            if (Constants.LostGroupInfo.Equals(id)) return;
            if (Constants.BuildinGroups.Any(b => b.ID == id)) return;
            SecurityContext.DemandPermissions(new[] { Constants.Action_EditGroups, Constants.Action_EditAz });

            userService.RemoveGroup(CoreContext.TenantManager.GetCurrentTenant().TenantId, id);
        }

        #endregion Groups


        private void RecursiveAddChildGroups(GroupInfo parent, List<GroupInfo> result)
        {
            if (parent == null || parent.Descendants == null) return;
            foreach (var group in parent.Descendants)
            {
                if (!result.Contains(group)) result.Add(group);
                RecursiveAddChildGroups(group, result);
            }
        }

        private bool IsPropertiesContainsWords(IEnumerable<string> properties, IEnumerable<string> words)
        {
            foreach (var w in words)
            {
                var find = false;
                foreach (var p in properties)
                {
                    find = p.StartsWith(w, StringComparison.CurrentCultureIgnoreCase);
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

        private IEnumerable<GroupInfo> GetGroupsInternal()
        {
            var groupsInfo = new Dictionary<Guid, GroupInfo>();
            var groups = userService.GetGroups(CoreContext.TenantManager.GetCurrentTenant().TenantId, default(DateTime)).ToDictionary(g => g.Id);
            foreach (var g in groups.Values.OrderBy(g => g.ParentId))
            {
                var gi = new GroupInfo
                {
                    ID = g.Id,
                    Name = g.Name,
                    CategoryID = g.CategoryId,
                };
                if (g.ParentId != Guid.Empty && groupsInfo.ContainsKey(g.ParentId))
                {
                    groupsInfo[g.ParentId].AddDescendant(gi);
                }
                groupsInfo[gi.ID] = gi;
            }
            return groupsInfo.Values.Concat(Constants.BuildinGroups);
        }

        private UserInfo ToUserInfo(User u)
        {
            if (u == null) return null;
            var ui = new UserInfo
            {
                BirthDate = u.BirthDate,
                Department = u.Department,
                Email = u.Email,
                FirstName = u.FirstName,
                ID = u.Id,
                LastName = u.LastName,
                Location = u.Location,
                Notes = u.Notes,
                Sex = u.Sex,
                Status = (EmployeeStatus)u.Status,
                TerminatedDate = u.WorkToDate,
                Title = u.Title,
                UserName = u.UserName,
                WorkFromDate = u.WorkFromDate,
            };
            ui.ContactsFromString(u.Contacts);
            return ui;
        }

        private User ToUser(UserInfo u)
        {
            if (u == null) return null;
            return new User
            {
                BirthDate = u.BirthDate,
                Contacts = u.ContactsToString(),
                Department = u.Department,
                Email = u.Email,
                FirstName = u.FirstName,
                Id = u.ID,
                LastName = u.LastName,
                Location = u.Location,
                Notes = u.Notes,
                Sex = u.Sex,
                Status = (int)u.Status,
                Title = u.Title,
                UserName = u.UserName,
                WorkFromDate = u.WorkFromDate,
                WorkToDate = u.TerminatedDate,
            };
        }

        private Group ToGroup(GroupInfo g)
        {
            if (g == null) return null;
            return new Group
            {
                Id = g.ID,
                Name = g.Name,
                ParentId = g.Parent != null ? g.Parent.ID : Guid.Empty,
                CategoryId = g.CategoryID,
            };
        }
    }
}