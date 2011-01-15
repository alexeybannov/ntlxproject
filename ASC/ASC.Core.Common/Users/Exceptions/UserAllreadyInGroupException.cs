#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class UserAllreadyInGroupException : RemotingException
    {
        public UserAllreadyInGroupException(UserInfo user, GroupInfo group)
            : base(string.Format(DescriptionResource.UserAllreadyInGroupException_Message, user, group))
        {
        }

        public UserAllreadyInGroupException(UserInfo user, GroupInfo group, GroupCategory groupCategory)
            : base(
                string.Format(DescriptionResource.UserAllreadyInGroupException_Message_Logical, groupCategory, user,
                              group))
        {
        }

        public UserAllreadyInGroupException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}