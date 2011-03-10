using System;

namespace ASC.Core.Notify
{
    [Serializable]
    public class NotifySenderDescription
    {
        public string ID { get; private set; }
        
        public string Name { get; private set; }

        
        public NotifySenderDescription(string id, string name)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("id");
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var desc = obj as NotifySenderDescription;
            return desc != null && desc.ID == ID;
        }
    }
}
