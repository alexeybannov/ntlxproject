#region usings

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using ASC.Common.Services;
using ASC.Core.Configuration;
using SmartAssembly.Attributes;

#endregion

namespace ASC.Core
{
    public class ClientConfiguration : IConfiguration
    {
        private readonly object syncRoot = new object();
        private int? secureCorePort;

        private readonly RemotingSubsystemConfiguration remotingSubsystemConfiguration =
            new RemotingSubsystemConfiguration();

        private SysConfig cfg;
        private bool? standalone;

        #region IService

        IServiceInfo IService.Info
        {
            [Obfuscation(Exclude = true)]
            [DoNotObfuscate]
            get { return CoreContext.InternalConfiguration.Info; }
        }

        #endregion

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
            [Obfuscation(Exclude = true)]
            [DoNotObfuscate]
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
            private string[] serviceTrustZone;
            private DateTime? installDate;
            private Guid? installID;

            internal SysConfig(ClientConfiguration clientCongfig)
            {
                if (clientCongfig == null) throw new ArgumentNullException("clientCongfig");
                config = clientCongfig;
            }

            #region settings

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

            public string[] ServiceTrustZone
            {
                get
                {
                    CheckAndFlushSettings();
                    if (serviceTrustZone == null)
                    {
                        var obj = config.GetSetting(Constants.CfgKey_ServiceTrustZone) as string;
                        serviceTrustZone = obj == null
                                               ? new[]
                                                     {
                                                         "localhost", Dns.GetHostName(),
                                                         Dns.GetHostEntry(Dns.GetHostName()).HostName
                                                     }
                                               : Deserialize<string[]>(obj);

                        if (!Array.Exists(serviceTrustZone, host => host == "localhost"))
                        {
                            var zone = new List<string>(serviceTrustZone);
                            zone.Add("localhost");
                            if (!Array.Exists(serviceTrustZone, host => host == Dns.GetHostName()))
                                zone.Add(Dns.GetHostName());
                            if (
                                !Array.Exists(serviceTrustZone,
                                              host => host == Dns.GetHostEntry(Dns.GetHostName()).HostName))
                                zone.Add(Dns.GetHostEntry(Dns.GetHostName()).HostName);
                            serviceTrustZone = zone.ToArray();
                        }
                    }
                    return (string[]) serviceTrustZone.Clone();
                }
                set
                {
                    if (value == null) throw new ArgumentNullException();
                    config.SaveSetting(Constants.CfgKey_ServiceTrustZone, Serialize(value));
                    serviceTrustZone = (string[]) value.Clone();
                }
            }

            public bool DemoAccountEnabled
            {
                get { return true; }
                set { }
            }

            public DateTime InstallDate
            {
                get
                {
                    if (!installDate.HasValue)
                    {
                        object obj = config.GetSetting(Constants.CfgKey_InstallDate);
                        installDate = obj == null ? new DateTime(2000, 01, 01) : Deserialize<DateTime>(obj as string);
                    }
                    return installDate.Value;
                }
            }

            public Guid InstallID
            {
                get
                {
                    if (!installID.HasValue)
                    {
                        object obj = config.GetSetting(Constants.CfgKey_InstallID);
                        installID = obj == null ? Guid.Empty : Deserialize<Guid>(obj as string);
                    }
                    return installID.Value;
                }
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
                if (type == typeof (DateTime))
                {
                    return DateTime.FromFileTime(Int64.Parse(value));
                }
                if (type == typeof (SmtpSettings))
                {
                    string[] props = value.Split(new[] {'#'}, StringSplitOptions.None);
                    props = Array.ConvertAll(props, p => !string.IsNullOrEmpty(p) ? p : null);
                    return new SmtpSettings
                               {
                                   CredentialsDomain = props[0],
                                   CredentialsUserName = props[1],
                                   CredentialsUserPassword = props[2],
                                   Host = props[3],
                                   Port = String.IsNullOrEmpty(props[4]) ? null : (int?) Int32.Parse(props[4]),
                                   SenderAddress = props[5],
                                   SenderDisplayName = props[6],
                                   EnableSSL =
                                       7 < props.Length && !string.IsNullOrEmpty(props[7])
                                           ? Convert.ToBoolean(props[7])
                                           : false
                               };
                }
                if (type == typeof (string[]))
                {
                    string[] props = value.Split(new[] {'#'}, StringSplitOptions.None);
                    return Array.ConvertAll(props, pr => String.IsNullOrEmpty(pr) ? null : pr);
                }
                if (type == typeof (Guid))
                {
                    return new Guid(value);
                }
                return Convert.ChangeType(value, type);
            }

            internal T Deserialize<T>(string value)
            {
                return (T) Deserialize(value, typeof (T));
            }

            #endregion

            public bool IsInTrustZone(string hostName)
            {
                return Array.Exists(ServiceTrustZone, host => string.Compare(host, hostName, true) == 0);
            }

            public bool IsInTrustZone(IPAddress ip)
            {
                if (ip == null) throw new ArgumentNullException("ip");
                string hostName = Dns.GetHostEntry(ip).HostName;
                return IsInTrustZone(hostName);
            }

            private void CheckAndFlushSettings()
            {
                if (DateTime.Now - lastFlushSettings > flushInterval)
                {
                    lastFlushSettings = DateTime.Now;
                    smtpSettings = null;
                    installDate = null;
                    installID = null;
                }
            }
        }

        #endregion
    }
}