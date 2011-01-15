#region usings

using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public interface IRoleProvider
    {
        List<IRole> GetRoles(ISubject account);
        List<ISubject> GetSubjects(IRole role);
        bool IsSubjectInRole(ISubject account, IRole role);
    }
}