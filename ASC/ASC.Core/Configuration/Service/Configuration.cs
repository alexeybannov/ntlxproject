using System;
using System.Collections;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration.Service;
using ASC.Core.Factories;

[assembly: AssemblyServices(typeof(Configuration))]

namespace ASC.Core.Configuration.Service
{
    [Locator]
    class Configuration : RemotingServiceController, IConfiguration
    {
        internal const string PrvtCoreKeyKey = "asc.core.rsaprivatekey";
        private readonly object syncRoot = new object();
        private readonly Hashtable configHash = new Hashtable();
        private IDAOFactory factory;
        private bool? standalone;

        public Configuration(IDAOFactory daoFactory)
            : base(Constants.ConfigurationServiceInfo)
        {
            if (daoFactory == null) throw new ArgumentNullException("daoFactory");
            factory = daoFactory;
        }

        #region IConfiguration Members

        /// <inheritdoc/>
        public bool Standalone
        {
            get
            {
                if (!standalone.HasValue)
                {
                    standalone = System.Configuration.ConfigurationManager.AppSettings["asc.core.tenants.base-domain"] == "localhost";
                }
                return standalone.Value;
            }
        }

        public int SecureCorePort
        {
            get { return CoreContext.Configuration.SecureCorePort; }
        }

        public void SaveSetting(string key, object value)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            SecurityContext.DemandPermissions(ASC.Core.Configuration.Constants.Action_Configure);

            _SaveSetting(key, value);
        }

        public object GetSetting(string key)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return _GetSetting(key);
        }
        #endregion

        protected override void StartWork()
        {
            _EnsureFirstStartConfiguration();
        }

        private void _EnsureFirstStartConfiguration()
        {
            var obj = _GetSetting(ASC.Core.Configuration.Constants.CfgKey_InstallDate);
            if (obj == null)
            {
                _SaveSetting(ASC.Core.Configuration.Constants.CfgKey_InstallDate, DateTime.UtcNow.ToFileTime().ToString());
                _SaveSetting(ASC.Core.Configuration.Constants.CfgKey_InstallID, Guid.NewGuid().ToString());
            }
        }

        private void _SaveSetting(string key, object value)
        {

            if (value != null && !(value is string)) throw new ArgumentException("value may be only string");

            byte[] data = value != null ? System.Text.Encoding.UTF8.GetBytes(value as string) : null;// ASC.Runtime.Serialization.BinarySerializer.Instance.Serialize(value);

            lock (syncRoot)
            {
                try
                {
                    data = Crypto.GetV(data, 2, true);

                    factory.GetConfigDao().SaveSettings(key, data);

                    if (configHash.Contains(key))
                        configHash[key] = value;
                    else
                        configHash.Add(key, value);
                }
                catch
                {
                    configHash.Clear();
                }

            }
        }

        private object _GetSetting(string key)
        {
            object setting = null;
            if (configHash.Contains(key))
                setting = configHash[key];
            else
            {
                byte[] data = factory.GetConfigDao().GetSettings(key);

                try
                {
                    data = Crypto.GetV(data, 2, false);

                    if (data != null)
                    {
                        setting = System.Text.Encoding.UTF8.GetString(data);
                        //setting = ASC.Runtime.Serialization.BinarySerializer.Instance.Deserialize(data);
                    }
                }
                catch
                {

                }

                lock (syncRoot)
                {
                    if (setting != null && !configHash.Contains(key))
                        configHash[key] = setting;
                }
            }
            return setting;
        }
    }
}
