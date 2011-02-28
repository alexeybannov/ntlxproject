using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

namespace ASC.Common.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<Ace> GetAcl(ISubject subject, IAction action);

        IEnumerable<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider);
    }
}