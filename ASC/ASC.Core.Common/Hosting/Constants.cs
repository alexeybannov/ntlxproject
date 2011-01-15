#region usings

using System;
using System.Reflection;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common;
using ASC.Net;
using SmartAssembly.Attributes;

#endregion

namespace ASC.Core.Hosting
{
    public sealed class Constants
    {
        [Obfuscation(Exclude = true)] [DoNotObfuscate] internal static readonly ServiceInfoBase CoreHostServiceInfo =
            new ServiceInfoBase(
                typeof (ICoreHost),
                "main core services host",
                null,
                "core_host",
                new Version(CoreConstResource.Version),
                new[] {TransportType.Ipc, TransportType.Tcp},
                UriUtil.BuildUri(
                    CoreConstResource.CoreUriPostfix,
                    "core_host"
                    )
                );

        public static readonly ServiceInfoBase ModuleHostServiceInfo =
            new ServiceInfoBase(
                typeof (IModuleHost),
                "services module parts host",
                null,
                "module_host",
                new Version(CoreConstResource.Version),
                new[] {TransportType.Tcp, TransportType.Ipc},
                UriUtil.BuildUri(
                    CoreConstResource.CoreUriPostfix,
                    "module_host"
                    )
                );

        public static readonly ServiceInfoBase[] HostingServices = new[]
                                                                       {
                                                                           CoreHostServiceInfo,
                                                                           ModuleHostServiceInfo
                                                                       };
    }
}