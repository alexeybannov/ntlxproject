using System;
using System.Collections.Generic;

namespace LitS3
{
    public class AccessControlList
    {
        public Owner Owner
        {
            get;
            private set;
        }

        public IDictionary<Grantee, Permission> Grants
        {
            get;
            private set;
        }


        public AccessControlList(Owner owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            Owner = owner;
            Grants = new Dictionary<Grantee, Permission>();

            SetGrant(new Grantee(GranteeType.User, Owner.Id, Owner.ToString()), Permission.None);
            SetGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AuthenticatedUsers", "Authenticated Users"), Permission.None);
            SetGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AllUsers", "Anonimous"), Permission.None);
        }


        public void SetGrant(Grantee grantee, Permission permission)
        {
            if (grantee == null) throw new ArgumentNullException("grantee");
            Grants[grantee] = permission;
        }

        public void AddGrant(Grantee grantee, Permission permission)
        {
            if (grantee == null) throw new ArgumentNullException("grantee");
            Grants[grantee] |= permission;
        }

        public void RemoveGrant(Grantee grantee, Permission permission)
        {
            if (grantee == null) throw new ArgumentNullException("grantee");
            Grants[grantee] ^= permission;
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
        FullControl = 16
    }
}
