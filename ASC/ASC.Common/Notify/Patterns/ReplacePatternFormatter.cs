#region usings

using System;

#endregion

namespace ASC.Notify.Patterns
{
    public sealed class ReplacePatternFormatter
        : PatternFormatterSkeleton
    {
        public const string DefaultPattern = @"[[]%(?<tagName>[a-zA-Z0-9_\-.]+)%[]]";

        public ReplacePatternFormatter()
            : base(DefaultPattern)
        {
        }

        public ReplacePatternFormatter(string tagPattern)
            : base(tagPattern)
        {
        }

        internal ReplacePatternFormatter(string tagPattern, bool formatMessage)
            : base(tagPattern, formatMessage)
        {
        }

        protected override string FormatText(string text, ITagValue[] tagsValues)
        {
            if (String.IsNullOrEmpty(text)) return text;
            string formattedText =
                _RegEx.Replace(text,
                               match =>
                                   {
                                       ITagValue value = Array.Find(tagsValues,
                                                                    tagValue =>
                                                                    tagValue.Tag.Name == match.Groups["tagName"].Value);
                                       if (value != null && value.Value != null)
                                           return Convert.ToString(value.Value);
                                       return match.Value;
                                   }
                    );
            return formattedText;
        }
    }
}