#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class AuthCategory : IAction
    {
        private List<Action> _CategoryActions = new List<Action>();

        protected AuthCategory()
        {
        }

        public AuthCategory(Guid id, string name, string description, string sysname, IEnumerable<Action> actions)
            : this(id, name, description, sysname)
        {
            if (actions == null) throw new ArgumentNullException("actions");
            _CategoryActions.AddRange(actions);
            _CategoryActions.ForEach(act => act.Category = this);
        }

        public AuthCategory(Guid id, string name, string description, string sysname)
            : this(id, name, description)
        {
            SysName = sysname;
        }

        public AuthCategory(Guid id, string name, string description)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("id");
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            ID = id;
            Name = name;
            Description = description;
        }

        #region IFixedIdentification

        public Guid ID { get; protected set; }

        public string Name { get; protected set; }

        public string Description { get; protected set; }

        #endregion

        public List<Action> CategoryActions
        {
            get { return new List<Action>(_CategoryActions); }
            protected set { _CategoryActions = new List<Action>(value); }
        }

        #region IAction Members

        public string SysName { get; protected set; }

        #endregion

        public void AddAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _CategoryActions.Add(action);
            action.Category = this;
        }

        public void RemoveAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");
            _CategoryActions.Remove(action);
            action.Category = null;
        }
    }
}