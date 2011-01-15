#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupNotFoundException
        : GroupManipulationException
    {
        public Guid GroupID { get; set; }

        public GroupNotFoundException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.GroupNotFoundException_Message,
                    id
                    )
                )
        {
            GroupID = id;
        }

        public GroupNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}