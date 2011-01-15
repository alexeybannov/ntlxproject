#region usings

using System;
using ASC.Core.Users;

#endregion

namespace ASC.Core
{
    public interface IUserManagerClient : IUserManager
    {
        #region users

        bool UserExists(Guid userID);

        UserInfo[] GetUsers(EmployeeStatus status);

        string[] GetUserNames(EmployeeStatus status);

        UserInfo GetUserByUserName(string userName);

        bool IsUserNameExists(string userName);

        UserInfo GetUserByEmail(string email);

        UserInfo[] Search(string text);

        UserInfo[] Search(string text, UserSearchType searchType);

        UserInfo[] Search(string text, EmployeeStatus status);

        UserInfo[] Search(string text, EmployeeStatus status, UserSearchType searchType);

        UserInfo[] Search(string text, EmployeeStatus status, Guid groupId);

        UserInfo[] Search(string text, EmployeeStatus status, Guid groupId, UserSearchType searchType);

        #endregion

        #region groups and users

        GroupInfo[] GetUserGroups(Guid userID);

        GroupInfo[] GetUserGroups(Guid userID, IncludeType includeType);

        GroupInfo[] GetUserGroups(Guid userID, Guid categoryID);

        bool IsUserInGroup(Guid userID, Guid groupID);

        UserInfo[] GetUsersByGroup(Guid groupID, IncludeType includeType);

        #endregion

        #region Company Info

        UserInfo GetCompanyCEO();

        void SetCompanyCEO(Guid userId);

        GroupInfo[] GetDepartments();

        Guid GetDepartmentManager(Guid deparmentID);

        bool IsDepartmentManager(Guid deparmentID, Guid userID);

        bool IsManager(Guid userID);

        bool IsManager(Guid managerID, Guid userID);

        #endregion
    }
}