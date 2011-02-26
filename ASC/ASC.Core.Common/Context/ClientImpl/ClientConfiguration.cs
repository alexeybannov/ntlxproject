using System;
using System.Configuration;
using System.Text;
using ASC.Core.Configuration;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public class ClientConfiguration
    {
        private readonly ITenantService tenantService;


        public bool Standalone
        {
            get { return ConfigurationManager.AppSettings["asc.core.tenants.base-domain"] == "localhost"; }
        }

        public int SecureCorePort
        {
            get;
            set;
        }

        public RemotingSubsystemConfiguration RemotingSubsystemConfiguration
        {
            get;
            private set;
        }

        public SysConfig Cfg
        {
            get;
            private set;
        }


        public ClientConfiguration(ITenantService tenantService)
        {
            this.tenantService = tenantService;
            RemotingSubsystemConfiguration = new RemotingSubsystemConfiguration();
            Cfg = new SysConfig(this);
        }


        public void SaveSetting(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            SecurityContext.DemandPermissions(Constants.Action_Configure);

            var data = value != null ? Crypto.GetV(Encoding.UTF8.GetBytes(value), 2, true) : null;
            tenantService.SetTenantSettings(Tenant.DEFAULT_TENANT, key, data);
        }

        public string GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key);
            return data != null ? Encoding.UTF8.GetString(Crypto.GetV(data, 2, false)) : null;
        }


        public class SysConfig
        {
            private readonly ClientConfiguration config;


            public SysConfig(ClientConfiguration clientCongfig)
            {
                config = clientCongfig;
            }

            public SmtpSettings SmtpSettings
            {
                get
                {
                    var obj = config.GetSetting(Constants.CfgKey_SmtpSettings) as string;
                    return Deserialize(obj);
                }
                set
                {
                    config.SaveSetting(Constants.CfgKey_SmtpSettings, Serialize(value));
                }
            }

            public bool DemoAccountEnabled
            {
                get { return true; }
            }

            private string Serialize(SmtpSettings smtp)
            {
                if (smtp == null) return null;
                return string.Join("#",
                    new[] {
                        smtp.CredentialsDomain,
                        smtp.CredentialsUserName,
                        smtp.CredentialsUserPassword,
                        smtp.Host,
                        smtp.Port.HasValue ? smtp.Port.Value.ToString() : string.Empty,
                        smtp.SenderAddress,
                        smtp.SenderDisplayName,
                        smtp.EnableSSL.ToString()});
            }

            private SmtpSettings Deserialize(string value)
            {
                if (string.IsNullOrEmpty(value)) return new SmtpSettings();

                var props = value.Split(new[] { '#' }, StringSplitOptions.None);
                props = Array.ConvertAll(props, p => !string.IsNullOrEmpty(p) ? p : null);
                return new SmtpSettings
                {
                    CredentialsDomain = props[0],
                    CredentialsUserName = props[1],
                    CredentialsUserPassword = props[2],
                    Host = props[3],
                    Port = String.IsNullOrEmpty(props[4]) ? null : (int?)Int32.Parse(props[4]),
                    SenderAddress = props[5],
                    SenderDisplayName = props[6],
                    EnableSSL = 7 < props.Length && !string.IsNullOrEmpty(props[7]) ? Convert.ToBoolean(props[7]) : false
                };
            }
        }
    }
}
