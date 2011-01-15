#region usings

using ASC.Net;

#endregion

namespace ASC.Core.Common.CoreTalk
{
    public interface ICoreAddressResolver
    {
        ConnectionHostEntry GetCoreHostEntry();
    }
}