using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Security.Authorizing
{
    class PermissionProvider : IPermissionProvider
    {
        public IEnumerable<Ace> GetAcl(ISubject subject, IAction action)
        {
            return CoreContext.AuthorizationManager
                .GetAces(subject.ID, action.ID)
                .Select(r => ToAce(r));
        }

        public IEnumerable<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            var filteredAces = CoreContext.AuthorizationManager
                .GetAllObjectAces(new[] { action }, objectId, secObjProvider)
                .Where(r => subject.ID == r.SubjectId && action.ID == r.ActionId);
            var aces = new List<Ace>();
            var aceKeys = new List<string>();
            foreach (var ace in filteredAces)
            {
                string key = string.Format("{0}{1:D}", ace.ActionId, ace.Reaction);
                if (!aceKeys.Contains(key))
                {
                    aceKeys.Add(key);
                    aces.Add(new Ace(ace.ActionId, ace.Reaction));
                }
            }
            return aces;
        }

        private Ace ToAce(AzRecord r)
        {
            return new Ace(r.ActionId, r.Reaction);
        }
    }
}