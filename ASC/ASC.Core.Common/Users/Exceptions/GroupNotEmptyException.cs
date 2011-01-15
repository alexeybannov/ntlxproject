#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupNotEmptyException
        : GroupManipulationException
    {
        public Guid GroupID { get; set; }

        public GroupNotEmptyException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.GroupNotEmptyException_Message,
                    id
                    )
                )
        {
            GroupID = id;
        }

        public GroupNotEmptyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}