#region usings

using System;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Users
{
    [Service("{E2C64542-46E5-4cc1-BD4B-748ED6F8B289}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    interface IUserManager : IService
    {
        #region users

        UserInfo[] GetUsers();

        UserInfo GetUsers(Guid userID);

        UserInfo SaveUserInfo(UserInfo userInfo);

        void DeleteUser(Guid userID);

        void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo);

        byte[] GetUserPhoto(Guid userID, Guid moduleID);

        #endregion

        #region groups and users

        UserGroupReference[] GetUsersGroupReferences();

        void AddUserIntoGroup(Guid userID, Guid groupID);

        void RemoveUserFromGroup(Guid userID, Guid groupID);

        #endregion

        #region Company Info

        DepartmentManagerRef[] GetDepartmentManagerRefs();

        void SetDepartmentManager(Guid deparmentID, Guid userID);

        bool RemoveDepartmentManager(Guid deparmentID, Guid userID);

        #endregion
    }
}