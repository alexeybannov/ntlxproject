#region usings

using System;
using System.Diagnostics;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    [DebuggerDisplay("ObjectType: {ObjectType.Name}, SecurityId: {SecurityId}")]
    public class SecurityObjectId : ISecurityObjectId
    {
        protected SecurityObjectId()
        {
        }

        public SecurityObjectId(object id, object obj)
            : this(id, obj.GetType())
        {
        }

        public SecurityObjectId(object id, Type objType)
        {
            if (objType == null) throw new ArgumentNullException("objType");
            SecurityId = id;
            ObjectType = objType;
        }

        #region ISecurityObjectId Members

        public virtual object SecurityId { get; protected set; }

        public virtual Type ObjectType { get; protected set; }

        #endregion

        public override int GetHashCode()
        {
            return AzObjectIdHelper.GetFullObjectId(this).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SecurityObjectId;
            return other != null &&
                   Equals(AzObjectIdHelper.GetFullObjectId(other), AzObjectIdHelper.GetFullObjectId(this));
        }
    }

    public class SecurityObjectId<T> : SecurityObjectId
    {
        public SecurityObjectId(object id)
            : base(id, typeof (T))
        {
        }
    }
}