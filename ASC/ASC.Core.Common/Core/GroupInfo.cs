#region usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ASC.Common.Security.Authorizing;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupInfo : IRole, IRecipientsGroup
    {
        public Guid ID { get; internal set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CategoryID { get; set; }

        public GroupInfo Parent { get; internal set; }

        public IList<GroupInfo> Descendants
        {
            get { return new ReadOnlyCollection<GroupInfo>(Childs); }
        }

        internal List<GroupInfo> Childs { get; private set; }
        internal Guid ParentID { get; set; }

        public GroupInfo()
        {
            Childs = new List<GroupInfo>();
        }

        public GroupInfo(Guid categoryID)
            : this()
        {
            CategoryID = categoryID;
        }

        public void AddDescendant(GroupInfo group)
        {
            if (group != null && !Childs.Contains(group))
            {
                group.Parent = this;
                group.CategoryID = CategoryID;
                Childs.Add(group);
            }
        }

        public void RemoveDescendant(GroupInfo group)
        {
            if (group != null && Childs.Remove(group))
            {
                group.Parent = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return ID != Guid.Empty ? ID.GetHashCode() : base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var g = obj as GroupInfo;
            if (g == null) return false;
            if (ID == Guid.Empty && g.ID == Guid.Empty) return ReferenceEquals(this, g);
            return g.ID == ID;
        }

        #region IRecipient Members

        string IRecipient.ID
        {
            get { return ID.ToString(); }
        }

        string IRecipient.Name
        {
            get { return Name; }
        }

        #endregion

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }

        #endregion
    }
}