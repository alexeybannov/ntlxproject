using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

namespace ASC.Common.Security
{
    public interface IRoleProvider
    {
        List<IRole> GetRoles(ISubject account);
        bool IsSubjectInRole(ISubject account, IRole role);
    }
}