#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class Action : IAction
    {
        internal Action()
        {
        }

        public Action(Guid id)
            : this(id, null, null)
        {
        }

        public Action(Guid id, string name, string description, string sysname)
            : this(id, name, description)
        {
            SysName = sysname;
        }

        public Action(Guid id, string name, string description)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("id");
            ID = id;
            Name = name;
            Description = description;
        }

        #region IFixedIdentification

        public Guid ID { get; internal set; }

        public string Name { get; internal set; }

        public string Description { get; internal set; }

        #endregion

        public AuthCategory Category { get; internal set; }

        #region IAction Members

        public string SysName { get; internal set; }

        #endregion

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var act = obj as Action;
            if (act == null) return false;
            return ID.Equals(act.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}