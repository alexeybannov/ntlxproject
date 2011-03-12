using System;
using System.Diagnostics;

namespace ASC.Core
{
    [DebuggerDisplay("{UserId} - {GroupId}")]
    public class UserGroupRef
    {
        public Guid UserId
        {
            get;
            set;
        }

        public Guid GroupId
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

        public UserGroupRefType RefType
        {
            get;
            set;
        }

        public int Tenant
        {
            get;
            set;
        }


        public UserGroupRef()
        {

        }

        public UserGroupRef(Guid userId, Guid groupId, UserGroupRefType refType)
        {
            UserId = userId;
            GroupId = groupId;
            RefType = refType;
        }


        public override int GetHashCode()
        {
            return UserId.GetHashCode() ^ GroupId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var r = obj as UserGroupRef;
            return r != null && r.UserId == UserId && r.GroupId == GroupId;
        }
    }
}
