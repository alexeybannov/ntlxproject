#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class AzCategoryNotFoundException
        : AzManipulationException
    {
        public Guid CategoryID { get; set; }

        public AzCategoryNotFoundException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.AzCategoryNotFoundException_Message,
                    id
                    )
                )
        {
            CategoryID = id;
        }

        public AzCategoryNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}