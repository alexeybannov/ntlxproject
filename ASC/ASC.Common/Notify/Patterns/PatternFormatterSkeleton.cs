#region usings

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Patterns
{
    public abstract class PatternFormatterSkeleton
        : IPatternFormatter
    {
        protected readonly Regex _RegEx;
        private readonly bool _formatMessage;

        protected string _TagSearchPattern;

        public PatternFormatterSkeleton()
        {
        }

        public PatternFormatterSkeleton(string tagSearchRegExp)
            : this(tagSearchRegExp, false)
        {
        }

        internal PatternFormatterSkeleton(string tagSearchRegExp, bool formatMessage)
        {
            if (String.IsNullOrEmpty(tagSearchRegExp)) throw new ArgumentException("tagSearchRegExp");
            _TagSearchPattern = tagSearchRegExp;
            _RegEx = new Regex(_TagSearchPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            _formatMessage = formatMessage;
        }

        #region IPatternFormatter

        public ITag[] GetTags(IPattern pattern)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            var findedTags = new List<ITag>(SearchTags(pattern.Body));
            Array.ForEach(
                SearchTags(pattern.Subject),
                tag => { if (!findedTags.Contains(tag)) findedTags.Add(tag); }
                );
            return findedTags.ToArray();
        }

        public void FormatMessage(INoticeMessage message, ITagValue[] tagsValues)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Pattern == null) throw new ArgumentException("message");
            if (tagsValues == null) throw new ArgumentNullException("tagsValues");
            BeforeFormat(message, tagsValues);

            message.Subject = FormatText(_formatMessage ? message.Subject : message.Pattern.Subject, tagsValues);

            message.Body = FormatText(_formatMessage ? message.Body : message.Pattern.Body, tagsValues);
            AfterFormat(message);
        }

        public void FormatMessage(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            FormatMessage(message, message.Arguments);
        }

        #endregion

        protected virtual void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues)
        {
        }

        protected virtual void AfterFormat(INoticeMessage message)
        {
        }

        protected abstract string FormatText(string text, ITagValue[] tagsValues);

        protected virtual ITag[] SearchTags(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(_TagSearchPattern))
                return new Tag[0];
            MatchCollection maches = _RegEx.Matches(text);
            var findedTags = new List<ITag>(maches.Count);
            foreach (Match mach in maches)
            {
                var tag = new Tag(mach.Groups["tagName"].Value);
                if (!findedTags.Contains(tag))
                    findedTags.Add(tag);
            }

            return findedTags.ToArray();
        }
    }
}