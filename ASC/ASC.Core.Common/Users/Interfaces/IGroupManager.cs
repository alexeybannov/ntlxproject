using System;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

namespace ASC.Core.Users
{
    [Service("{086E079B-D179-4927-89F1-68B6974A05C7}", ServiceInstancingType.Singleton)]
    [ChannelDemand]
    interface IGroupManager : IService
    {
        #region Groups

        [AuthenticationLevel(SecurityLevel.None)]
        GroupInfo[] GetGroups();

        GroupInfo GetGroupInfo(Guid groupID);

        GroupInfo SaveGroupInfo(GroupInfo groupInfo);

        void DeleteGroup(Guid groupID);

        #endregion
    }
}