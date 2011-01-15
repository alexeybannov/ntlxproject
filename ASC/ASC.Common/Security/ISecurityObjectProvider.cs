#region usings

using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public interface ISecurityObjectProvider
    {
        bool InheritSupported { get; }

        bool ObjectRolesSupported { get; }
        ISecurityObjectId InheritFrom(ISecurityObjectId objectId);

        IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext);
    }
}