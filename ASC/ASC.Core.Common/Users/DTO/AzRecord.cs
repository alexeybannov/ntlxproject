#region usings

using System;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class AzRecord : ICloneable
    {
        public Guid SubjectId { get; private set; }

        public Guid ActionId { get; private set; }

        public AceType Reaction { get; private set; }

        public bool Inherited { get; internal set; }

        public string FullObjectId { get; private set; }

        public string Id { get; private set; }

        public string Creator { get; set; }

        public AzRecord(Guid subjectId, Guid actionId, AceType reaction)
            : this(subjectId, actionId, reaction, default(string))
        {
        }

        public AzRecord(Guid subjectId, Guid actionId, AceType reaction, ISecurityObjectId objectId)
            : this(subjectId, actionId, reaction, AzObjectIdHelper.GetFullObjectId(objectId))
        {
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var r = obj as AzRecord;
            return r != null && Equals(r.Id, Id);
        }

        public override string ToString()
        {
            return Id;
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        internal static AzRecord Parse(string ace)
        {
            string[] parts = ace.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            return new AzRecord(
                new Guid(parts[0]),
                new Guid(parts[1]),
                (AceType) int.Parse(parts[2]),
                3 < parts.Length ? parts[3] + '|' + parts[4] : null
                );
        }

        internal AzRecord(Guid subjectId, Guid actionId, AceType reaction, string fullObjectId)
        {
            SubjectId = subjectId;
            ActionId = actionId;
            Reaction = reaction;
            Inherited = false;
            FullObjectId = fullObjectId;
            Id = string.Format("{0}|{1}|{2:D}|{3}", SubjectId, ActionId, Reaction, FullObjectId);
        }
    }
}