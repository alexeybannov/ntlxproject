using System;
using ASC.Core.Configuration;

namespace ASC.Core
{
    public class ClientConfiguration
    {
        private readonly object syncRoot = new object();
        private int? secureCorePort;

        private readonly RemotingSubsystemConfiguration remotingSubsystemConfiguration = new RemotingSubsystemConfiguration();

        private SysConfig cfg;
        private bool? standalone;


        #region IConfiguration

        public bool Standalone
        {
            get
            {
                if (!standalone.HasValue)
                {
                    standalone = CoreContext.InternalConfiguration.Standalone;
                }
                return standalone.Value;
            }
        }

        public int SecureCorePort
        {
            get
            {
                if (!secureCorePort.HasValue) secureCorePort = CoreContext.InternalConfiguration.SecureCorePort;
                return secureCorePort.Value;
            }
            internal set { secureCorePort = value; }
        }

        public void SaveSetting(string key, object value)
        {
            CoreContext.InternalConfiguration.SaveSetting(key, value);
        }

        public object GetSetting(string key)
        {
            return CoreContext.InternalConfiguration.GetSetting(key);
        }

        #endregion


        public RemotingSubsystemConfiguration RemotingSubsystemConfiguration
        {
            get { return remotingSubsystemConfiguration; }
        }

        #region settings

        public SysConfig Cfg
        {
            get
            {
                if (cfg == null) cfg = new SysConfig(this);
                return cfg;
            }
        }

        public class SysConfig
        {
            private readonly ClientConfiguration config;
            private DateTime lastFlushSettings = DateTime.Now;
            private readonly TimeSpan flushInterval = TimeSpan.FromMinutes(1);
            private SmtpSettings smtpSettings;


            internal SysConfig(ClientConfiguration clientCongfig)
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

            #endregion

            #region serialization of properties in line

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

            #endregion

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