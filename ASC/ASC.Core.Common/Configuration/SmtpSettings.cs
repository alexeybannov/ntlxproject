using System;

namespace ASC.Core.Configuration
{
    [Serializable]
    public class SmtpSettings
    {
        public string Host;

        public int? Port;

        public string SenderAddress;

        public string SenderDisplayName;

        public string CredentialsDomain;

        public string CredentialsUserName;

        public string CredentialsUserPassword;

        public bool EnableSSL;

        public override bool Equals(object obj)
        {
            var settings = obj as SmtpSettings;
            if (settings == null) return false;
            return
                string.Equals(Host, settings.Host) &&
                string.Equals(SenderAddress, settings.SenderAddress) &&
                string.Equals(SenderDisplayName, settings.SenderDisplayName) &&
                string.Equals(CredentialsDomain, settings.CredentialsDomain) &&
                string.Equals(CredentialsUserName, settings.CredentialsUserName) &&
                string.Equals(CredentialsUserPassword, settings.CredentialsUserPassword) &&
                EnableSSL == settings.EnableSSL &&
                Port == settings.Port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
