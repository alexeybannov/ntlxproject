#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class ServiceDefinitionException : RemotingException
    {
        public ServiceDefinitionException(string serviceName)
            : base(String.Format(CommonDescriptionResource.ServiceDefinitionException_Message, serviceName))
        {
        }

        public ServiceDefinitionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}