using System;
using ASC.Core.Users;

namespace ASC.Core
{
    public interface IGroupManagerClient
    {
        GroupInfo[] GetGroups();

        GroupInfo[] GetGroups(Guid categoryID);

        GroupInfo GetGroupInfo(Guid groupID);

        GroupInfo SaveGroupInfo(GroupInfo groupInfo);

        void DeleteGroup(Guid groupID);
    }
}
