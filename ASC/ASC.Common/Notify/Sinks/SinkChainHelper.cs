namespace ASC.Notify.Sinks
{
    internal sealed class SinkChainHelper
    {
        public static ISink AddSink(ISink firstSink, ISink addedSink)
        {
            if (firstSink == null) return addedSink;
            if (addedSink == null) return firstSink;
            ISink current = firstSink;
            while (current.NextSink != null) current = current.NextSink;
            current.NextSink = addedSink;
            return firstSink;
        }

        public static ISinkProvider AddSinkProvider(ISinkProvider first, ISinkProvider added)
        {
            if (first == null) return added;
            if (added == null) return first;
            ISinkProvider current = first;
            while (current.Next != null) current = current.Next;
            current.Next = added;
            return first;
        }
    }
}