#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
using AuthConst = ASC.Common.Security.Authorizing.Constants;
using ConfConst = ASC.Core.Configuration.Constants;

#endregion

namespace ASC.Core.Security.Authorizing
{
    internal class RoleProvider : IRoleProvider
    {
        public List<ISubject> GetSubjects(IRole role)
        {
            if (role == null) throw new ArgumentNullException("role");
            if (role.Equals(AuthConst.Demo)) return new List<ISubject> {ConfConst.Demo};
            return
                GetParentRoles(role.ID)
                    .Select(r => (ISubject) r)
                    .Union(GetUsersInRole(role.ID))
                    .ToList();
        }

        public List<IRole> GetRoles(ISubject account)
        {
            var roles = new List<IRole>();
            if (account is ISysAccount)
            {
                if (ConfConst.Demo.Equals(account)) roles.Add(AuthConst.Demo);
            }
            else
            {
                if (account is IRole)
                {
                    roles = GetParentRoles(account.ID).ToList();
                }
                else if (account is IUserAccount)
                {
                    roles = CoreContext.UserManager
                        .GetUserGroups(account.ID, IncludeType.Distinct | IncludeType.InParent)
                        .Select(g => (IRole) g)
                        .ToList();
                }
            }
            return roles;
        }

        public bool IsSubjectInRole(ISubject account, IRole role)
        {
            return CoreContext.UserManager.IsUserInGroup(account.ID, role.ID);
        }

        private List<IRole> GetParentRoles(Guid roleID)
        {
            var roles = new List<IRole>();
            GroupInfo gi = CoreContext.GroupManager.GetGroupInfo(roleID);
            if (gi != null)
            {
                GroupInfo parent = gi.Parent;
                while (parent != null)
                {
                    roles.Add(parent);
                    parent = parent.Parent;
                }
            }
            return roles;
        }

        private IEnumerable<ISubject> GetUsersInRole(Guid roleId)
        {
            return CoreContext.UserManager
                .GetUsersByGroup(roleId, IncludeType.Distinct | IncludeType.InChild)
                .Select(u => (ISubject) u);
        }
    }
}