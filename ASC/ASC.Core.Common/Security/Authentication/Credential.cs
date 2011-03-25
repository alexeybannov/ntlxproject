using System;

namespace ASC.Core.Security.Authentication
{
    [Serializable]
    class Credential
    {
        public Credential(string login, string passwordHash)
            : this(login, passwordHash, -1)
        {
        }

        public Credential(string login, string passwordHash, int tenant)
        {
            Login = login;
            PasswordHash = passwordHash;
            Tenant = tenant;
        }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public int Tenant { get; set; }

        public override bool Equals(object obj)
        {
            var cr = obj as Credential;
            return cr != null && cr.Login == Login && cr.PasswordHash == PasswordHash && cr.Tenant == Tenant;
        }

        public override int GetHashCode()
        {
            return (Login ?? string.Empty).GetHashCode() | (PasswordHash ?? string.Empty).GetHashCode() | Tenant;
        }
    }
}