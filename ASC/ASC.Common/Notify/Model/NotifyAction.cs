#region usings

using System;

#endregion

namespace ASC.Notify.Model
{
    [Serializable]
    public class NotifyAction
        : INotifyAction
    {
        public NotifyAction(string id, string name)
        {
            if (id == null) throw new ArgumentNullException("id");
            ID = id;
            Name = name;
        }

        #region INotifyAction 

        public string ID { get; private set; }

        public string Name { get; private set; }

        #endregion

        public override bool Equals(object obj)
        {
            var action = obj as INotifyAction;
            if (obj == null) return false;
            else
                return ID == action.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("action:{0}", Name);
        }
    }
}