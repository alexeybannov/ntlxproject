#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class UserManipulationException
        : RemotingException
    {
        public UserManipulationException(string message)
            : base(message)
        {
        }

        public UserManipulationException(string message, string systemMessage)
            : base(message)
        {
            SystemMessage = systemMessage;
        }

        public UserManipulationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            SystemMessage = info.GetString("SystemMessage");
        }

        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("SystemMessage", SystemMessage);
            base.GetObjectData(info, context);
        }

        public string SystemMessage { get; set; }
    }
}