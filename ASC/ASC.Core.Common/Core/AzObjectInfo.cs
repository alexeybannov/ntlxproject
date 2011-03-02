#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Core
{
    [DebuggerDisplay("ObjectId = {ObjectId}, InheritAces = {InheritAces}")]
    [Serializable]
    public class AzObjectInfo
    {
        public static readonly bool DefaultInheritAce = true;

        public string ObjectId { get; private set; }

        public bool InheritAces { get; set; }
        private readonly IDictionary<string, RoleSubjectPair> roleSubjects;

        internal AzObjectInfo(string objectId)
        {
            if (objectId == null) throw new ArgumentNullException("objectId");
            ObjectId = objectId;
            InheritAces = DefaultInheritAce;
            roleSubjects = new Dictionary<string, RoleSubjectPair>();
        }

        public AzObjectInfo(ISecurityObjectId objectId)
            : this(AzObjectIdHelper.GetFullObjectId(objectId))
        {
        }

        public override int GetHashCode()
        {
            return ObjectId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var oi = obj as AzObjectInfo;
            return oi != null && ObjectId.Equals(oi.ObjectId);
        }

        public void AddRoleSubject(IRole role, ISubject subject)
        {
            var roleSubject = new RoleSubjectPair(role, subject);
            if (!roleSubjects.ContainsKey(roleSubject.Id))
            {
                roleSubjects.Add(roleSubject.Id, roleSubject);
            }
        }

        public bool RemoveRoleSubject(IRole role, ISubject subject)
        {
            string id = new RoleSubjectPair(role, subject).Id;
            if (roleSubjects.ContainsKey(id))
            {
                return roleSubjects.Remove(id);
            }
            return false;
        }

        public IList<RoleSubjectPair> GetRoleSubjects()
        {
            return new List<RoleSubjectPair>(roleSubjects.Values);
        }

        public IList<IRole> GetRolesBySubject(ISubject subject)
        {
            var roles = new List<IRole>();
            foreach (RoleSubjectPair r in roleSubjects.Values)
            {
                if (r.Subject.Equals(subject) || ContainsInSelf(r.Subject, subject)) roles.Add(r.Role);
            }
            return roles;
        }

        private bool ContainsInSelf(ISubject container, ISubject subject)
        {
            var groups = CoreContext.UserManager.GetUserGroups(subject.ID);
            return Array.Exists(groups, g => { return g.ID.Equals(container.ID); });
        }

        public IList<ISubject> GetSubjectsByRole(IRole role)
        {
            var subjects = new List<ISubject>();
            foreach (RoleSubjectPair r in roleSubjects.Values)
            {
                if (r.Role.Equals(role)) subjects.Add(r.Subject);
            }
            return subjects;
        }

        public void ClearRoleSubjects()
        {
            roleSubjects.Clear();
        }

        public void ClearRolesBySubject(ISubject subject)
        {
            if (subject == null) return;
            var copy = new Dictionary<string, RoleSubjectPair>(roleSubjects);
            foreach (KeyValuePair<string, RoleSubjectPair> entry in copy)
            {
                if (entry.Value.Subject.Equals(subject))
                {
                    roleSubjects.Remove(entry);
                }
            }
        }

        public void ClearSubjectsByRole(IRole role)
        {
            if (role == null) return;
            var copy = new Dictionary<string, RoleSubjectPair>(roleSubjects);
            foreach (KeyValuePair<string, RoleSubjectPair> entry in copy)
            {
                if (entry.Value.Role.Equals(role))
                {
                    roleSubjects.Remove(entry);
                }
            }
        }

        [Serializable]
        public class RoleSubjectPair
        {
            internal string Id { get; private set; }

            public IRole Role { get; private set; }

            public ISubject Subject { get; private set; }

            public RoleSubjectPair(IRole role, ISubject subject)
            {
                if (role == null) throw new ArgumentNullException("role");
                if (subject == null) throw new ArgumentNullException("subject");
                Role = role;
                Subject = subject;
                Id = string.Format("{0}|{1}", Role.ID, Subject.ID);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var rs = obj as RoleSubjectPair;
                return rs != null && rs.Id == Id;
            }
        }
    }
}