#region usings

using System;

#endregion

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantOwner : IEquatable<TenantOwner>
    {
        public Guid Id { get; internal set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public TenantOwner(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");
            Id = Guid.NewGuid();
            Email = email;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TenantOwner);
        }

        public bool Equals(TenantOwner other)
        {
            return other != null && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} <{2}>", FirstName, LastName, Email);
        }
    }
}