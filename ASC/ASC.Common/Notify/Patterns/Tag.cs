#region usings

using System;

#endregion

namespace ASC.Notify.Patterns
{
    [Serializable]
    public class Tag
        : ITag
    {
        public Tag(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");
            Name = name;
        }

        #region ITag 

        public string Name { get; private set; }

        #endregion

        public override bool Equals(object obj)
        {
            var tag = obj as ITag;
            if (tag == null) return false;
            return String.Equals(Name, tag.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0}]", Name);
        }
    }
}