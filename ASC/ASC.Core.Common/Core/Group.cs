using System;

namespace ASC.Core
{
    public class Group
    {
        public Guid Id
        {
            get;
            set;
        }

        public Guid ParentId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Guid CategoryId
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }

        public DateTime ModifiedOn
        {
            get;
            set;
        }


        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var g = obj as Group;
            return g != null && g.Id == Id;
        }
    }
}
