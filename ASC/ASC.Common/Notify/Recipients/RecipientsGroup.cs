#region usings

using System;

#endregion

namespace ASC.Notify.Recipients
{
    [Serializable]
    public class RecipientsGroup
        : IRecipientsGroup
    {
        public RecipientsGroup(string id, string name)
        {
            ID = id;
            Name = name;
        }

        #region IRecipientsGroup Members

        public string ID { get; private set; }

        public string Name { get; private set; }

        #endregion

        public override bool Equals(object obj)
        {
            var recGr = obj as IRecipientsGroup;
            if (recGr == null) return false;
            return Equals(recGr.ID, ID);
        }

        public override int GetHashCode()
        {
            return (ID ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }
    }
}