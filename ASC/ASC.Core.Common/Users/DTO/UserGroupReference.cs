#region usings

using System;
using System.Reflection;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class UserGroupReference
    {
        public UserGroupReference(Guid userId, Guid groupId)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException("userId");
            if (groupId == Guid.Empty) throw new ArgumentNullException("groupId");
            User = userId;
            Group = groupId;
            Id = string.Format("{0}|{1}", User, Group);
        }

        public Guid User { get; private set; }

        public Guid Group { get; private set; }

        internal string Id { get; private set; }

        public override bool Equals(object obj)
        {
            var r = obj as UserGroupReference;
            return r != null && Equals(Id, r.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", User, Group);
        }
    }
}