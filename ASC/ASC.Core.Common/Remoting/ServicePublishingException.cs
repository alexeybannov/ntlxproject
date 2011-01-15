#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Common.Remoting
{
    [Serializable]
    public class ServicePublishingException : RemotingException
    {
        public ServicePublishingException(string srvName, Exception innerException)
            :
                base(
                String.Format(CommonDescriptionResource.ServicePublishingException_default, srvName), innerException
                )
        {
        }

        public ServicePublishingException(string srvName)
            : this(srvName, (Exception) null)
        {
        }

        public ServicePublishingException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public ServicePublishingException(string srvName, string message)
            :
                base(message)
        {
        }
    }
}