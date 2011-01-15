namespace ASC.Notify.Patterns
{
    public interface ITagValue
    {
        ITag Tag { get; }

        object Value { get; set; }
    }
}