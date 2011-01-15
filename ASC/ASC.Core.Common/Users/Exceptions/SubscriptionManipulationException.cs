#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class SubscriptionManipulationException
        : RemotingException
    {
        public SubscriptionManipulationException(string message)
            : base(message)
        {
        }

        public SubscriptionManipulationException(string message, string systemMessage)
            : base(message)
        {
            SystemMessage = systemMessage;
        }

        public SubscriptionManipulationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public string SystemMessage { get; set; }
    }
}