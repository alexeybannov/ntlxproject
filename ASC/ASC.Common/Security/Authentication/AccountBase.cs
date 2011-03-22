#region usings

using System;

#endregion

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public abstract class AccountBase : IAccount
    {
        protected AccountBase(Guid id, string name)
        {
            ID = id;
            Name = name;
        }

        #region IAccount Members

        public Guid ID { get; private set; }

        public string Name { get; private set; }


        public object Clone()
        {
            return MemberwiseClone();
        }

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public virtual bool IsAuthenticated
        {
            get { return true; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var a = obj as IAccount;
            return a != null && ID.Equals(a.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}