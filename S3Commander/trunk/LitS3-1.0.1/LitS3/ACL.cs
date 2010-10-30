using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace LitS3
{
    public class AccessControlList
    {
        private List<Grant> grants = new List<Grant>();


        public Identity Owner
        {
            get;
            private set;
        }

        public IList<Grant> Grants
        {
            get { return grants.AsReadOnly(); }
        }


        public AccessControlList(Identity owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            Owner = owner;

            AddGrant(new Grantee(GranteeType.User, Owner.Id, Owner.ToString()), Permission.None);
            AddGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AuthenticatedUsers", "Authenticated Users"), Permission.None);
            AddGrant(new Grantee(GranteeType.Group, "http://acs.amazonaws.com/groups/global/AllUsers", "Anonimous"), Permission.None);
        }

        internal AccessControlList(XmlReader reader)
        {
            if (reader.IsEmptyElement) throw new Exception("Expected a non-empty <AccessControlPolicy> element.");

            reader.ReadStartElement("AccessControlPolicy");
            if (reader.Name == "Owner") Owner = new Identity(reader);

            reader.ReadStartElement("AccessControlList");
            while (reader.Name == "Grant")
            {
                AddGrant(new Grantee(reader), ParsePemission(reader.ReadElementContentAsString()));
            }

            reader.ReadEndElement();
            reader.ReadEndElement();
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


        private Permission ParsePemission(string permission)
        {
            switch (permission)
            {
                case "FULL_CONTROL": return Permission.FullControl;
                case "READ": return Permission.Read;
                case "READ_ACP": return Permission.ReadAcp;
                case "WRITE": return Permission.Write;
                case "WRITE_ACP": return Permission.WriteAcp;
                default: throw new ArgumentOutOfRangeException("Unknown permission: " + permission);
            }
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


    public class Grantee : Identity
    {
        public GranteeType GranteeType
        {
            get;
            private set;
        }


        public Grantee(GranteeType granteeType, string id)
            : this(granteeType, id, null)
        {

        }

        public Grantee(GranteeType granteeType, string id, string displayName)
            : base(id, displayName)
        {
            GranteeType = granteeType;
        }

        internal Grantee(XmlReader reader)
        {
            reader.ReadStartElement();

            var type = reader.GetAttribute("type", "xsi:type");
            Id = reader.ReadElementContentAsString();
            switch (type)
            {
                case "Group":
                    GranteeType = GranteeType.Group;
                    if (Id == "http://acs.amazonaws.com/groups/global/AuthenticatedUsers") DisplayName = "Authenticated Users";
                    if (Id == "http://acs.amazonaws.com/groups/global/AllUsers") DisplayName = "Anonimous";
                    break;

                case "CanonicalUser":
                    GranteeType = GranteeType.User;
                    break;

                case "EMail":
                    GranteeType = GranteeType.User;
                    break;
            }
            
            DisplayName = reader.ReadElementContentAsString("DisplayName", "");
            reader.ReadEndElement();
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
