#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class AzManipulationException
        : RemotingException
    {
        private string _SystemMessage;

        public AzManipulationException(string message)
            : base(message)
        {
        }

        public AzManipulationException(string message, string systemMessage)
            : base(message)
        {
            SystemMessage = systemMessage;
        }

        public AzManipulationException(
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