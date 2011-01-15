#region usings

using System;
using ASC.Net;

#endregion

namespace ASC.Core.Common.CoreTalk
{
    public sealed class FixedCoreAddressResolver : ICoreAddressResolver
    {
        private readonly ConnectionHostEntry _CoreHost;

        public const int Priority = 1000;

        public FixedCoreAddressResolver(ConnectionHostEntry coreHost)
        {
            if (coreHost == null) throw new ArgumentNullException("coreHost");
            _CoreHost = coreHost;
        }

        #region ICoreAddressResolver Members

        public ConnectionHostEntry GetCoreHostEntry()
        {
            return _CoreHost;
        }

        #endregion
    }
}