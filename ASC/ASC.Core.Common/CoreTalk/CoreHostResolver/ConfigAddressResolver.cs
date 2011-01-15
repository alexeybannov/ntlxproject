#region usings

using System.Configuration;
using ASC.Net;

#endregion

namespace ASC.Core.Common.CoreTalk
{
    internal class ConfigAddressResolver : ICoreAddressResolver
    {
        #region ICoreAddressResolver

        public ConnectionHostEntry GetCoreHostEntry()
        {
            try
            {
                string value = ConfigurationManager.AppSettings["coreport"];
                if (!string.IsNullOrEmpty(value))
                {
                    return new ConnectionHostEntry("localhost", int.Parse(ConfigurationManager.AppSettings["coreport"]));
                }
                value = ConfigurationManager.AppSettings["asc.core.address"];
                if (!string.IsNullOrEmpty(value))
                {
                    return new ConnectionHostEntry(value);
                }
            }
            catch
            {
            }
            return null;
        }

        #endregion
    }
}