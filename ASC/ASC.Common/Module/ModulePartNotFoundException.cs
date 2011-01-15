#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Module
{
    [Serializable]
    public class ModulePartNotFoundException : RemotingException
    {
        private readonly Guid _ID;

        public ModulePartNotFoundException(Guid id)
            : base(String.Format(CommonDescriptionResource.ModulePartNotFoundException_MessageByID, id))
        {
            _ID = id;
        }

        public ModulePartNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Guid ModulePartID
        {
            get { return _ID; }
        }
    }
}