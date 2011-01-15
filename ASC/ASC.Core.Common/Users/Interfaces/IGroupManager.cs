using System;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

namespace ASC.Core.Users
{
    [Service("{086E079B-D179-4927-89F1-68B6974A05C7}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    public interface IGroupManager : IService
    {
        #region category groups

        [AuthenticationLevel(SecurityLevel.None)]
        GroupCategory[] GetGroupCategories();

        GroupCategory MainGroupCategory { get; }

        GroupCategory[] GetGroupCategories(Guid moduleID);

        GroupCategory GetGroupCategory(Guid categoryID);

        GroupCategory SaveGroupCategory(GroupCategory category);

        void DeleteGroupCategory(Guid categoryID);

        #endregion

        #region Groups

        [AuthenticationLevel(SecurityLevel.None)]
        GroupInfo[] GetGroups();

        GroupInfo GetGroupInfo(Guid groupID);

        GroupInfo SaveGroupInfo(GroupInfo groupInfo);

        void DeleteGroup(Guid groupID);

        #endregion
    }
}