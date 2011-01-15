#region usings

using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public class SecurityCallContext
    {
        public SecurityCallContext()
        {
            ObjectsStack = new List<ISecurityObjectId>();
            RolesList = new List<IRole>();
        }

        public List<ISecurityObjectId> ObjectsStack { get; private set; }

        public List<IRole> RolesList { get; private set; }

        public object UserData { get; set; }
    }
}