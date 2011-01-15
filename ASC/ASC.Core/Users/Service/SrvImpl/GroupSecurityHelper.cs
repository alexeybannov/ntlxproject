using System;
using System.Collections.Generic;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Core.Users.Service.SrvImpl
{
	static class GroupSecurityHelper
	{
		public static void DemandPermission(GroupInfo groupInfo)
		{
			if (groupInfo == null) throw new ArgumentNullException("groupInfo");

			var actions = new List<Action>();

			actions.Add(Constants.Action_EditGroups);
			actions.Add(Constants.Action_EditAz);

			SecurityContext.DemandPermissions(actions.ToArray());
		}
	}
}