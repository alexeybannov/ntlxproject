#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class ServiceHostingModelException : RemotingException
    {
        public ServiceHostingModelException(string serviceName)
            : base(String.Format(CommonDescriptionResource.ServiceHostingModelException_Message, serviceName))
        {
        }

        public ServiceHostingModelException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}