#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class ServiceInstancingException : RemotingException
    {
        public ServiceInstancingException(string serviceName)
            :
                base(String.Format(CommonDescriptionResource.ServiceInstancingException_Message, serviceName))
        {
        }

        public ServiceInstancingException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}