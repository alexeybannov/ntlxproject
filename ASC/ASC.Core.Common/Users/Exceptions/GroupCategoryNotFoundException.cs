#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupCategoryNotFoundException
        : GroupCategoryManipulationException
    {
        public Guid GroupCategoryID { get; set; }

        public GroupCategoryNotFoundException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.GCNotFoundException_Message,
                    id
                    )
                )
        {
            GroupCategoryID = id;
        }

        public GroupCategoryNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}