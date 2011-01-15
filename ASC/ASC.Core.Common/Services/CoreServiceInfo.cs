#region usings

using System;
using ASC.Common.Services;
using ASC.Net;

#endregion

namespace ASC.Core.Common.Services
{
    [Serializable]
    internal class CoreServiceInfo : ServiceInfoBase
    {
        public CoreServiceInfo(Type type, string name, string description, string sysName, Version version,
                               TransportType[] transportTypes, string uri)
            : base(type, name, description, sysName, version, transportTypes, uri)
        {
        }
    }
}