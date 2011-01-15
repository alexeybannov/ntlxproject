#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public class AzManager
    {
        private readonly IPermissionProvider permissionProvider;
        private readonly IRoleProvider roleProvider;

        #region

        internal AzManager()
        {
        }

        public AzManager(IRoleProvider roleProvider, IPermissionProvider permissionProvider)
            : this()
        {
            if (roleProvider == null) throw new ArgumentNullException("roleProvider");
            if (permissionProvider == null) throw new ArgumentNullException("permissionProvider");
            this.roleProvider = roleProvider;
            this.permissionProvider = permissionProvider;
        }

        #endregion

        public bool CheckPermission(ISubject subject, IAction action, ISecurityObjectId objectId,
                                    ISecurityObjectProvider securityObjProvider, out ISubject denySubject,
                                    out IAction denyAction)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (action == null) throw new ArgumentNullException("action");
            AzManagerAcl acl = GetAzManagerAcl(subject, action, objectId, securityObjProvider);
            denySubject = acl.DenySubject;
            denyAction = acl.DenyAction;
            return acl.IsAllow;
        }

        internal AzManagerAcl GetAzManagerAcl(ISubject subject, IAction action)
        {
            return GetAzManagerAcl(subject, action, null, null);
        }

        internal AzManagerAcl GetAzManagerAcl(ISubject subject, IAction action, ISecurityObjectId objectId,
                                              ISecurityObjectProvider securityObjProvider)
        {
            if (Constants.Admin.ID == subject.ID || roleProvider.IsSubjectInRole(subject, Constants.Admin))
            {
                return AzManagerAcl.Allow;
            }
            AzManagerAcl acl = AzManagerAcl.Default;
            bool findDeny = false;
            foreach (ISubject subj in GetSubjects(subject, objectId, securityObjProvider))
            {
                List<Ace> aceList = (objectId == null)
                                        ? permissionProvider.GetAcl(subj, action)
                                        : permissionProvider.GetAcl(subj, action, objectId, securityObjProvider);
                foreach (Ace ace in aceList)
                {
                    if (ace.Reaction == AceType.Deny && !findDeny)
                    {
                        findDeny = true;
                        acl.IsAllow = false;
                        acl.DenySubject = subj;
                        acl.DenyAction = action;
                    }
                    if (ace.Reaction == AceType.Allow && !findDeny)
                    {
                        acl.IsAllow = true;
                    }
                    if (findDeny) break;
                }
                if (findDeny) break;
            }
            return acl;
        }

        internal IEnumerable<ISubject> GetSubjects(ISubject subject)
        {
            return GetSubjects(subject, null, null);
        }

        internal IEnumerable<ISubject> GetSubjects(ISubject subject, ISecurityObjectId objectId,
                                                   ISecurityObjectProvider securityObjProvider)
        {
            var subjects = new List<ISubject>();
            subjects.Add(subject);
            subjects.AddRange(
                roleProvider.GetRoles(subject)
                    .ConvertAll(r => { return (ISubject) r; })
                );
            if (objectId != null)
            {
                var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, securityObjProvider);
                do
                {
                    if (!secObjProviderHelper.ObjectRolesSupported) continue;
                    foreach (IRole role in secObjProviderHelper.GetObjectRoles(subject))
                    {
                        if (!subjects.Contains(role)) subjects.Add(role);
                    }
                } while (secObjProviderHelper.NextInherit());
            }
            return subjects;
        }

        #region Nested type: AzManagerAcl

        internal class AzManagerAcl
        {
            public IAction DenyAction;
            public ISubject DenySubject;
            public bool IsAllow;

            public static AzManagerAcl Allow
            {
                get { return new AzManagerAcl {IsAllow = true}; }
            }

            public static AzManagerAcl Default
            {
                get { return new AzManagerAcl {IsAllow = false}; }
            }
        }

        #endregion
    }
}