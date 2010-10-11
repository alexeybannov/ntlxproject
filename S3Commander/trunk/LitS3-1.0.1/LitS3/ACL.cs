using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LitS3
{
    public class AccessControlList
    {
        private List<Grant> grants = new List<Grant>();


        public Owner Owner
        {
            get;
            private set;
        }

        public IList<Grant> Grants
        {
            get { return grants.AsReadOnly(); }
        }


        public AccessControlList(Owner owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            Owner = owner;

            AddGrant(new Grantee(GranteeType.User, Owner.Id, Owner.ToString()), Permission.None);
            AddGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AuthenticatedUsers", "Authenticated Users"), Permission.None);
            AddGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AllUsers", "Anonimous"), Permission.None);
        }

        public void AddGrant(Grantee grantee, Permission permission)
        {
            if (FindGrant(grantee) != null)
            {
                throw new ArgumentException(string.Format("Grantee {0} exists.", grantee));
            }
            grants.Add(new Grant(grantee, permission));
        }

        public Grant FindGrant(Grantee grantee)
        {
            return grants.Find(g => grantee.Equals(g.Grantee));
        }

        public void RemoveGrant(Grantee grantee)
        {
            grants.RemoveAll(g => grantee.Equals(g.Grantee));
        }
    }


    public class Owner
    {
        public string Id
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }


        public Owner(string displayName, string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            Id = id;
            DisplayName = displayName;
        }


        public override bool Equals(object obj)
        {
            var owner = obj as Owner;
            return owner != null && owner.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} (Owner)", DisplayName ?? Id);
        }
    }


    [DebuggerDisplay("Grantee = {Grantee}, Permission = {Permission}")]
    public class Grant
    {
        public Grantee Grantee
        {
            get;
            private set;
        }

        public Permission Permission
        {
            get;
            private set;
        }

        public Grant(Grantee grantee, Permission permission)
        {
            Grantee = grantee;
            Permission = permission;
        }

        public void AddPermission(Permission permission)
        {
            Permission |= permission;
        }

        public void RemovePermission(Permission permission)
        {
            if ((Permission & permission) == permission)
            {
                Permission ^= permission;
            }
        }
    }


    public class Grantee
    {
        public GranteeType GranteeType
        {
            get;
            private set;
        }

        public string Id
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }


        public Grantee(GranteeType granteeType, string id)
            : this(granteeType, id, null)
        {

        }

        public Grantee(GranteeType granteeType, string id, string displayName)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            GranteeType = granteeType;
            Id = id;
            DisplayName = displayName;
        }


        public override bool Equals(object obj)
        {
            var grantee = obj as Grantee;
            return grantee != null && grantee.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return DisplayName ?? Id;
        }
    }

    public enum GranteeType
    {
        User,
        Email,
        Group,
    }


    [Flags]
    public enum Permission
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadAcp = 4,
        WriteAcp = 8,
        FullControl = 16,
        Default = None
    }
}
