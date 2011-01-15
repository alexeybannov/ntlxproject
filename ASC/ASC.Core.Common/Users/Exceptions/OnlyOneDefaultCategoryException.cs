#region usings

using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Users.Resources;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class OnlyOneDefaultCategoryException : GroupCategoryManipulationException
    {
        public OnlyOneDefaultCategoryException()
            : base(
                DescriptionResource.OODCException_Message
                )
        {
        }

        protected OnlyOneDefaultCategoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}