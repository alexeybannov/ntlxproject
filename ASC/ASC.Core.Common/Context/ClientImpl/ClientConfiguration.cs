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
            if (value == null) throw new ArgumentNullException("value");
            SecurityContext.DemandPermissions(Constants.Action_Configure);

            var data = value != null ? Encoding.UTF8.GetBytes(value) : null;
            data = Crypto.GetV(data, 2, true);
            tenantService.SetTenantSettings(Tenant.DEFAULT_TENANT, key, data);
        }

        public string GetSetting(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            var data = tenantService.GetTenantSettings(Tenant.DEFAULT_TENANT, key);
            data = Crypto.GetV(data, 2, false);
            return data != null ? Encoding.UTF8.GetString(data) : null;
        }


        public class SysConfig
        {
            private readonly ClientConfiguration config;
            private readonly TimeSpan flushInterval = TimeSpan.FromMinutes(1);
            private DateTime lastFlushSettings = DateTime.Now;
            private SmtpSettings smtpSettings;


            public SysConfig(ClientConfiguration clientCongfig)
            {
                if (clientCongfig == null) throw new ArgumentNullException("clientCongfig");
                config = clientCongfig;
            }

            public SmtpSettings SmtpSettings
            {
                get
                {
                    CheckAndFlushSettings();
                    if (smtpSettings == null)
                    {
                        var obj = config.GetSetting(Constants.CfgKey_SmtpSettings) as string;
                        smtpSettings = obj == null ? new SmtpSettings() : Deserialize<SmtpSettings>(obj);
                    }
                    return smtpSettings;
                }
                set
                {
                    if (value == null) throw new ArgumentNullException();
                    config.SaveSetting(Constants.CfgKey_SmtpSettings, Serialize(value));
                    smtpSettings = value;
                }
            }

            public bool DemoAccountEnabled
            {
                get { return true; }
                set { }
            }

            internal string Serialize(SmtpSettings smtpSettings)
            {
                if (smtpSettings == null) return null;
                return String.Join("#",
                                   new[]
                                       {
                                           smtpSettings.CredentialsDomain,
                                           smtpSettings.CredentialsUserName,
                                           smtpSettings.CredentialsUserPassword,
                                           smtpSettings.Host,
                                           smtpSettings.Port.HasValue ? smtpSettings.Port.Value.ToString() : "",
                                           smtpSettings.SenderAddress,
                                           smtpSettings.SenderDisplayName,
                                           smtpSettings.EnableSSL.ToString()
                                       }
                    );
            }

            internal string Serialize(string[] property)
            {
                if (property == null) return null;
                return String.Join("#", property);
            }

            internal string Serialize(object property)
            {
                if (property == null) return null;
                if (property is string && String.IsNullOrEmpty(property as string)) return null;
                return property.ToString();
            }

            internal string Serialize(DateTime datetime)
            {
                return datetime.ToFileTime().ToString();
            }

            internal object Deserialize(string value, Type type)
            {
                if (string.IsNullOrEmpty(value) || type == null) return null;
                if (type == typeof(DateTime))
                {
                    return DateTime.FromFileTime(Int64.Parse(value));
                }
                if (type == typeof(SmtpSettings))
                {
                    string[] props = value.Split(new[] { '#' }, StringSplitOptions.None);
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
                                   EnableSSL =
                                       7 < props.Length && !string.IsNullOrEmpty(props[7])
                                           ? Convert.ToBoolean(props[7])
                                           : false
                               };
                }
                if (type == typeof(string[]))
                {
                    string[] props = value.Split(new[] { '#' }, StringSplitOptions.None);
                    return Array.ConvertAll(props, pr => String.IsNullOrEmpty(pr) ? null : pr);
                }
                if (type == typeof(Guid))
                {
                    return new Guid(value);
                }
                return Convert.ChangeType(value, type);
            }

            internal T Deserialize<T>(string value)
            {
                return (T)Deserialize(value, typeof(T));
            }

            private void CheckAndFlushSettings()
            {
                if (DateTime.Now - lastFlushSettings > flushInterval)
                {
                    lastFlushSettings = DateTime.Now;
                    smtpSettings = null;
                }
            }
        }
    }
}