#region usings

using System;

#endregion

namespace ASC.Notify.Patterns
{
    public class DynamicTagValue : ITagValue
    {
        private readonly Func<object> value;

        public DynamicTagValue(ITag tag, Func<object> getValue)
        {
            if (getValue == null) throw new ArgumentNullException("getValue");
            if (tag == null) throw new ArgumentNullException("tag");
            Tag = tag;
            value = getValue;
        }

        public DynamicTagValue(string tagName, Func<object> getValue)
            : this(new Tag(tagName), getValue)
        {
        }

        #region ITagValue Members

        public ITag Tag { get; private set; }

        public object Value
        {
            get { return value(); }
            set { throw new InvalidOperationException(); }
        }

        #endregion
    }
}