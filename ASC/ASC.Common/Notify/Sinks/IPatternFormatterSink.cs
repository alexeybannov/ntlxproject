#region usings

using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Sinks
{
    public interface IPatternFormatterSink
        : ISink
    {
        IPatternFormatter PatternFormatter { get; set; }
    }
}