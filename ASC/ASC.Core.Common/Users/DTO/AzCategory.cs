#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Security.Authorizing;
using Action = ASC.Common.Security.Authorizing.Action;

#endregion

namespace ASC.Core.Users
{
    [Serializable]
    public class AzCategory : AuthCategory
    {
        public AzCategory(AuthCategory authCategory, Guid moduleID)
        {
            ID = authCategory.ID;
            Name = authCategory.Name;
            Description = authCategory.Description;
            SysName = authCategory.SysName;
            ModuleID = moduleID;
            SetActions(authCategory.CategoryActions);
        }

        public AzCategory(Guid id, string name, string description, string sysname, Guid moduleID)
        {
            ID = id;
            Name = name;
            Description = description;
            SysName = sysname;
            ModuleID = moduleID;
        }

        public Guid ModuleID { get; internal set; }

        public void SetActions(List<Action> actions)
        {
            CategoryActions = new List<Action>(actions);
        }
    }
}