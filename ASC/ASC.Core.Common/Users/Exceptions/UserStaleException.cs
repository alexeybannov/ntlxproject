#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class UserStaleException
        : UserManipulationException
    {
        private readonly UserInfo _userInfo;

        public UserInfo NewUserInfo
        {
            get { return _userInfo; }
        }

        public UserStaleException(UserInfo realUserInfo)
            : base(DescriptionResource.UserStaleException_Message)
        {
            _userInfo = realUserInfo;
        }

        public UserStaleException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            _userInfo = (UserInfo) info.GetValue("_userInfo", typeof (UserInfo));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_userInfo", _userInfo);
            base.GetObjectData(info, context);
        }
    }
}