using System;
using System.Collections.Generic;

namespace ASC.Core
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers(int tenant, DateTime from);

        User GetUser(int tenant, Guid id);

        User SaveUser(int tenant, User user);

        void RemoveUser(int tenant, Guid id);

        byte[] GetUserPhoto(int tenant, Guid id);

        void SetUserPhoto(int tenant, Guid id, byte[] photo);


        IEnumerable<Group> GetGroups(int tenant, DateTime from);

        Group GetGroup(int tenant, Guid id);

        Group SaveGroup(int tenant, Group group);

        void RemoveGroup(int tenant, Guid id);


        IEnumerable<UserGroupRef> GetUserGroupRefs(int tenant, DateTime from);

        UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r);

        void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);
    }
}
