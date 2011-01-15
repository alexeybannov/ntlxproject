#region usings

using System;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Sinks
{
    internal class PatternFormatterSinkImpl
        : SinkSkeleton, IPatternFormatterSink
    {
        #region ISink

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            IPatternFormatter patternFormatter = PatternFormatter;
            if (patternFormatter == null)
                throw new ApplicationException("pattern formatter not instanced");

            patternFormatter.FormatMessage(message);
            return NextSink.ProcessMessage(message);
        }

        #endregion

        #region IPatternFormatterSink

        public IPatternFormatter PatternFormatter { get; set; }

        #endregion
    }
}