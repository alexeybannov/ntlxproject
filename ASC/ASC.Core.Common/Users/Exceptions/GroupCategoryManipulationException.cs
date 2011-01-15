#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupCategoryManipulationException
        : RemotingException
    {
        public GroupCategoryManipulationException(string message)
            : base(message)
        {
        }

        public GroupCategoryManipulationException(string message, string systemMessage)
            : base(message)
        {
            SystemMessage = systemMessage;
        }

        public GroupCategoryManipulationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public string SystemMessage { get; set; }
    }
}