#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Common.Services
{
    [Serializable]
    public class ModuleHostNotFoundException
        : RemotingException
    {
        private readonly Guid _ID;

        public ModuleHostNotFoundException(Guid moduleHostID)
            :
                base(String.Format(CommonDescriptionResource.ModuleHostNotFoundException_Message, moduleHostID))
        {
            _ID = moduleHostID;
        }

        public ModuleHostNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public Guid ModuleHostID
        {
            get { return _ID; }
        }
    }
}