#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Runtime.Remoting.Channels
{
    public interface IChannelWithServices
    {
        IList<Guid> Services { get; }
        void AttachService(Guid serviceID);
        void DeattachService(Guid serviceID);
    }
}