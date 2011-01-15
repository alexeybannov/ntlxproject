#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupManipulationException
        : RemotingException
    {
        private string _SystemMessage;

        public GroupManipulationException(string message)
            : base(message)
        {
        }

        public GroupManipulationException(string message, string systemMessage)
            : base(message)
        {
            SystemMessage = systemMessage;
        }

        public GroupManipulationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public string SystemMessage
        {
            get { return _SystemMessage; }
            set { _SystemMessage = value; }
        }
    }
}