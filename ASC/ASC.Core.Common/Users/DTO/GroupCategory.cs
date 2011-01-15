#region usings

using System;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupCategory
    {
        internal GroupCategory()
        {
        }

        public GroupCategory(Guid moduleID)
        {
            ModuleID = moduleID;
        }

        public Guid ID { get; internal set; }

        public Guid ModuleID { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public GroupType GroupType { get; set; }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var gc = obj as GroupCategory;
            return gc != null && ID == gc.ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}