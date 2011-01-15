namespace ASC.Notify.Patterns
{
    public interface IPatternProvider
    {
        IPattern GetPattern(string id);

        IPattern[] GetPatterns();

        IPatternFormatter GetFormatter(IPattern pattern);
    }
}