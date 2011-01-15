#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class AzActionNotFoundException
        : AzManipulationException
    {
        public Guid CategoryID { get; set; }

        public AzActionNotFoundException(Guid id)
            : base(
                String.Format(
                    DescriptionResource.AzActionNotFoundException_Message,
                    id
                    )
                )
        {
            CategoryID = id;
        }

        public AzActionNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}