#region usings

using System.Threading;

#endregion

namespace System.IO
{
    public static class StreamExtensions
    {
        private const int maxBufferSize = 8192;

        public static void Copy(this Stream source, Stream output)
        {
            IAsyncResult reader = null;
            var readerBuffer = new byte[maxBufferSize];
            IAsyncResult writer = null;
            byte[] writerBuffer = null;
            int length = source.Read(readerBuffer, 0, maxBufferSize);
            while (length != 0)
            {
                writerBuffer = new byte[length];
                Array.Copy(readerBuffer, writerBuffer, length);
                writer = output.BeginWrite(writerBuffer, 0, length, null, null);
                reader = source.BeginRead(readerBuffer, 0, maxBufferSize, null, null);
                WaitHandle.WaitAll(new[]
                                       {
                                           reader.AsyncWaitHandle,
                                           writer.AsyncWaitHandle
                                       });
                output.EndWrite(writer);
                length = source.EndRead(reader);
            }
        }
    }
}