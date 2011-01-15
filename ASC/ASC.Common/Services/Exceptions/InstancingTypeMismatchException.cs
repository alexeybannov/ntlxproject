#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public class InstancingTypeMismatchException : RemotingException
    {
        public InstancingTypeMismatchException(string serviceName)
            :
                base(String.Format(CommonDescriptionResource.InstancingTypeMismatchException_Message, serviceName))
        {
        }

        public InstancingTypeMismatchException(string serviceName, string reason)
            :
                base(
                String.Format(
                    CommonDescriptionResource.InstancingTypeMismatchException_Message + "[reason:{1}]"
                    , serviceName
                    , reason
                    ))
        {
        }

        public InstancingTypeMismatchException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}