#region usings

using System;
using System.Reflection;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common;
using ASC.Net;

#endregion

namespace ASC.Core.Hosting
{
    public sealed class Constants
    {
        internal static readonly ServiceInfoBase CoreHostServiceInfo =
            new ServiceInfoBase(
            typeof(ICoreHost),
            "main core services host",
            null,
            "core_host",
            new Version("0.1"),
            new[] { TransportType.Ipc, TransportType.Tcp },
            UriUtil.BuildUri("core_host")
            );

        public static readonly ServiceInfoBase ModuleHostServiceInfo =
            new ServiceInfoBase(
                typeof(IModuleHost),
                "services module parts host",
                null,
                "module_host",
                new Version("0.1"),
                new[] { TransportType.Tcp, TransportType.Ipc },
                UriUtil.BuildUri("module_host")
                );

        public static readonly ServiceInfoBase[] HostingServices = new[]
                                                                       {
                                                                           CoreHostServiceInfo,
                                                                           ModuleHostServiceInfo
                                                                       };
    }
}