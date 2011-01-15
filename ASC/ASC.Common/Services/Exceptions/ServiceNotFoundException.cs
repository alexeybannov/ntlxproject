#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class ServiceNotFoundException : RemotingException
    {
        public ServiceNotFoundException(string serviceName)
            :
                base(String.Format(CommonDescriptionResource.ServiceNotFoundException_Message, serviceName))
        {
        }

        public ServiceNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}