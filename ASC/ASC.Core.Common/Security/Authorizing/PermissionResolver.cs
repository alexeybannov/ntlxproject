#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using Constants = ASC.Core.Configuration.Constants;

#endregion

namespace ASC.Core.Security.Authorizing
{
    internal class PermissionResolver : IPermissionResolver
    {
        private readonly AzManager azManager;
        private IPermissionProvider permissionProvider;

        public PermissionResolver(AzManager azManager, IPermissionProvider permissionProvider)
        {
            if (azManager == null) throw new ArgumentNullException("azManager");
            if (permissionProvider == null) throw new ArgumentNullException("permissionProvider");
            this.azManager = azManager;
            this.permissionProvider = permissionProvider;
        }

        #region IPermissionResolver

        public bool Check(ISubject subject, params IAction[] actions)
        {
            return Check(subject, null, null, actions);
        }

        public bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider,
                          params IAction[] actions)
        {
            DenyResult[] denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
            return denyActions.Length == 0;
        }

        public void Demand(ISubject subject, params IAction[] actions)
        {
            Demand(subject, null, null, actions);
        }

        public void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider,
                           params IAction[] actions)
        {
            DenyResult[] denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
            if (0 < denyActions.Length)
            {
                throw new AuthorizingException(
                    subject,
                    Array.ConvertAll(denyActions, r => r.TargetAction),
                    Array.ConvertAll(denyActions, r => r.DenySubject),
                    Array.ConvertAll(denyActions, r => r.DenyAction)
                    );
            }
        }

        #endregion

        private DenyResult[] GetDenyActions(ISubject subject, IAction[] actions, ISecurityObjectId objectId,
                                            ISecurityObjectProvider securityObjProvider)
        {
            var denyActions = new List<DenyResult>();
            if (actions == null) actions = new IAction[0];
            if (subject == null)
            {
                denyActions = actions.Select(a => new DenyResult(a)).ToList();
            }
            else if (subject is ISysAccount && subject.ID == Constants.CoreSystem.ID)
            {
            }
            else
            {
                ISubject denySubject = null;
                IAction denyAction = null;
                foreach (IAction action in actions)
                {
                    bool allow = azManager.CheckPermission(subject, action, objectId, securityObjProvider,
                                                           out denySubject, out denyAction);
                    if (!allow)
                    {
                        denyActions.Add(new DenyResult(action, denySubject, denyAction));
                        break;
                    }
                }
            }
            return denyActions.ToArray();
        }

        private class DenyResult
        {
            public readonly IAction TargetAction;
            public readonly ISubject DenySubject;
            public readonly IAction DenyAction;

            public DenyResult(IAction targetAction)
            {
                TargetAction = targetAction;
            }

            public DenyResult(IAction targetAction, ISubject denySubject, IAction denyAction)
                : this(targetAction)
            {
                DenySubject = denySubject;
                DenyAction = denyAction;
            }
        }
    }
}