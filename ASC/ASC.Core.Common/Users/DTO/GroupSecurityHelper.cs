using System.Collections.Generic;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Core.Users
{
	static class GroupSecurityHelper
	{
		public static void DemandPermission()
		{
			var actions = new List<Action>();

			actions.Add(Constants.Action_EditGroups);
			actions.Add(Constants.Action_EditAz);

			SecurityContext.DemandPermissions(actions.ToArray());
		}
	}
}