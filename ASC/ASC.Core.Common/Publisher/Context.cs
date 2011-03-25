#region usings

using System;

#endregion

namespace ASC.Core.Common.Publisher
{
    public class Context
    {
        internal Func<IUserManagerClient> GetUserManagerClient { get; set; }
        internal Func<IConfigurationClient> GetConfigurationClient { get; set; }
        private IUserManagerClient _instanceUM;
        private IConfigurationClient _instanceC;

        public IUserManagerClient UserManager
        {
            get
            {
                if (_instanceUM != null) return _instanceUM;
                if (GetUserManagerClient != null)
                    return GetUserManagerClient();
                else
                    return null;
            }
            set { _instanceUM = value; }
        }

        public IConfigurationClient Configuration
        {
            get
            {
                if (_instanceC != null) return _instanceC;
                if (GetConfigurationClient != null)
                    return GetConfigurationClient();
                else
                    return null;
            }
            set { _instanceC = value; }
        }
    }
}