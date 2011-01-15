#region usings

using System;

#endregion

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
                String.Equals(Host, settings.Host) &&
                String.Equals(SenderAddress, settings.SenderAddress) &&
                String.Equals(SenderDisplayName, settings.SenderDisplayName) &&
                String.Equals(CredentialsDomain, settings.CredentialsDomain) &&
                String.Equals(CredentialsUserName, settings.CredentialsUserName) &&
                String.Equals(CredentialsUserPassword, settings.CredentialsUserPassword) &&
                Equals(EnableSSL, settings.EnableSSL) &&
                (Equals(Port, settings.Port));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}