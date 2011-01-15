#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class UserNotFoundException
        : UserManipulationException
    {
        public Guid UserID { get; set; }

        public UserNotFoundException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.UserNotFoundException_Message,
                    id
                    )
                )
        {
            UserID = id;
        }

        public UserNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}