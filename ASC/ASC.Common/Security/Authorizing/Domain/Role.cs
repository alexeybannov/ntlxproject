#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public sealed class Role : IRole
    {
        public const string Everyone = "Everyone";
        public const string Visitors = "Visitors";
        public const string Users = "Users";
        public const string Administrators = "Administrators";
        public const string System = "System";

        public Role(Guid id, string name, string description)
        {
            if (id == Guid.Empty) throw new ArgumentException("id");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            ID = id;
            Name = name;
            Description = description;
        }

        #region IFixedIdentification

        public string Description { get; internal set; }
        public Guid ID { get; internal set; }

        public string Name { get; internal set; }

        #endregion

        #region ISubject

        public SubjectType Type
        {
            get { return SubjectType.Role; }
        }

        #endregion

        #region IRole Members

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }

        #endregion

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var role = obj as Role;
            return role != null && ID.Equals(role.ID);
        }

        public override string ToString()
        {
            return string.Format("Role: {0}", Name);
        }
    }
}