#region usings

using System.Net.Security;
using ASC.Common.Services;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Hosting
{
    [Service("{ac681e0c-ec26-4b10-bbe9-4b196111d273}", ServiceInstancingType.MultipleInstances)]
    [ChannelDemand(ProtectionLevel.EncryptAndSign)]
    public interface IModuleHost : IInstancedService, IServicesModulePartsHost
    {
        void Remove();
    }
}