#region usings

using System;
using System.Net.Security;
using System.Security.Principal;

#endregion

namespace ASC.Runtime.Remoting.Channels
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple = false, Inherited = true)]
    public class ChannelDemandAttribute : Attribute
    {
        public ChannelDemandAttribute()
        {
            ChannelDemand = new ChannelDemand();
        }

        public ChannelDemandAttribute(ProtectionLevel protectionLevel)
        {
            ChannelDemand = new ChannelDemand(protectionLevel);
        }

        public ChannelDemandAttribute(ProtectionLevel protectionLevel, TokenImpersonationLevel impersonationLevel,
                                      string channelName)
        {
            ChannelDemand = new ChannelDemand(protectionLevel, impersonationLevel, channelName);
        }

        public ChannelDemand ChannelDemand { get; private set; }
    }
}