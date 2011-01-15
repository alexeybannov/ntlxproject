#region usings

using System;

#endregion

namespace ASC.Notify.Patterns
{
    public class TagValue
        : ITagValue
    {
        public TagValue(string tagName)
            : this(tagName, null)
        {
        }

        public TagValue(string tagName, object value)
            : this(new Tag(tagName), value)
        {
        }

        public TagValue(ITag tag, object value)
        {
            if (tag == null) throw new ArgumentNullException("tag");
            Tag = tag;
            Value = value;
        }

        #region ITagValue

        public ITag Tag { get; private set; }

        public object Value { get; set; }

        #endregion
    }
}