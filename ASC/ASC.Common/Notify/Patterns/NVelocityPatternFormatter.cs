#region usings

using System;
using System.IO;
using System.Text;
using ASC.Common.Collections;
using ASC.Common.Utils;
using ASC.Common.Utils.Extentions;
using ASC.Notify.Messages;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime.Resource.Loader;

#endregion

namespace ASC.Notify.Patterns
{
    

    public sealed class NVelocityPatternFormatter
        : PatternFormatterSkeleton
    {
        public const string DefaultPattern = @"(^|[^\\])\$[\{]{0,1}(?<tagName>[a-zA-Z0-9_]+)";
        private VelocityContext _nvelocityContext;

        public NVelocityPatternFormatter()
            : base(DefaultPattern)
        {
        }

        protected override void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues)
        {
            _nvelocityContext = new VelocityContext();
            foreach (ITagValue tagValue in tagsValues)
                _nvelocityContext.Put(tagValue.Tag.Name, tagValue.Value);
            base.BeforeFormat(message, tagsValues);
        }

        protected override string FormatText(string text, ITagValue[] tagsValues)
        {
            if (String.IsNullOrEmpty(text)) return text;
            return VelocityFormatter.FormatText(text,_nvelocityContext);
        }

        protected override void AfterFormat(INoticeMessage message)
        {
            _nvelocityContext = null;
            base.AfterFormat(message);
        }
    }
}