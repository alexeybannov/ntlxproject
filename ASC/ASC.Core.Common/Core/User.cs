using System;
using System.Diagnostics;

namespace ASC.Core
{
    [DebuggerDisplay("{LastName} {FirstName}")]
    public class User
    {
        public Guid Id
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public DateTime? BirthDate
        {
            get;
            set;
        }

        public bool? Sex
        {
            get;
            set;
        }

        public int Status
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Department
        {
            get;
            set;
        }

        public DateTime? WorkFromDate
        {
            get;
            set;
        }

        public DateTime? WorkToDate
        {
            get;
            set;
        }

        public string Contacts
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string Notes
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public int Tenant
        {
            get;
            set;
        }


        public override string ToString()
        {
            return UserName;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var u = obj as User;
            return u != null && u.Id == Id;
        }
    }
}
