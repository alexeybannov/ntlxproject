#region usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace ASC.Runtime.Serialization
{
    public sealed class BinarySerializer
    {
        private static BinarySerializer instance;
        private readonly object syncRoot = new object();
        private BinaryFormatter formatter;

        private BinaryFormatter Formatter
        {
            get
            {
                if (formatter == null)
                {
                    lock (syncRoot)
                    {
                        if (formatter == null) formatter = new BinaryFormatter();
                    }
                }
                return formatter;
            }
        }

        public static BinarySerializer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof (BinarySerializer))
                    {
                        if (instance == null) instance = new BinarySerializer();
                    }
                }
                return instance;
            }
        }

        public byte[] Serialize(object obj)
        {
            var ms = new MemoryStream();
            Serialize(obj, ms);
            var data = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(data, 0, data.Length);
            return data;
        }

        public void Serialize(object obj, Stream stream)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            if (!stream.CanWrite) throw new ArgumentException("stream");
            Formatter.Serialize(stream, obj);
        }

        public object Deserialize(Stream stream)
        {
            return Formatter.Deserialize(stream);
        }

        public object Deserialize(byte[] data)
        {
            var ms = new MemoryStream(data);
            ms.Seek(0, SeekOrigin.Begin);
            return Formatter.Deserialize(ms);
        }
    }
}