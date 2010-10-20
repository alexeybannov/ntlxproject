using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NXmlConnector.Properties;

namespace NXmlConnector
{
    static class NXmlConnector
    {
        private static bool started = false;

        private static ConnectorCallBack callback;

        private readonly static Encoding utf8 = Encoding.UTF8;


        public static string SendCommand(string command)
        {
            StartUp();

            var buffer = utf8.GetBytes(command);
            var ptr = Marshal.AllocHGlobal(buffer.Length);
            try
            {
                Marshal.Copy(buffer, 0, ptr, buffer.Length);
                var result = SendCommand(ptr);
                return PtrToString(result);
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
            }
        }

        public static event Action<string> NewData;


        private static void StartUp()
        {
            if (!started)
            {
                lock (typeof(NXmlConnector))
                {
                    if (!started)
                    {
                        callback = new ConnectorCallBack(CallBack);
                        if (SetCallback(callback))
                        {
                            started = true;
                        }
                        else
                        {
                            throw new NXmlConnectorException(Resources.CanNotSetCallback);
                        }
                    }
                }
            }
        }

        private static void CallBack(IntPtr pData)
        {
            var data = PtrToString(pData);

            var newData = NewData;
            if (newData != null)
            {
                newData(data);
            }
        }


        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr SendCommand(IntPtr pData);

        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern bool FreeMemory(IntPtr pData);

        [DllImport("txmlconnector.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern bool SetCallback([MarshalAs(UnmanagedType.FunctionPtr)]ConnectorCallBack pCallback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate void ConnectorCallBack(IntPtr pData);


        private static string PtrToString(IntPtr ptr)
        {
            try
            {
                var offset = 0;
                var buffer = new MemoryStream(255);
                while (true)
                {
                    var b = Marshal.ReadByte(ptr, offset);
                    if (b == 0 || offset == int.MaxValue) break;
                    
                    buffer.WriteByte(b);
                    offset++;
                }
                return utf8.GetString(buffer.ToArray());
            }
            finally
            {
                FreeMemory(ptr);
            }
        }
    }
}
