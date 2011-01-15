#region usings

using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public interface IPermissionProvider
    {
        List<Ace> GetAcl(IAction action);

        List<Ace> GetAcl(ISubject subject);

        List<Ace> GetAcl(ISubject subject, IAction action);

        List<Ace> GetAcl(ISubject subject, IAction action, ISecurityObjectId objectId,
                         ISecurityObjectProvider secObjProvider);
    }
}