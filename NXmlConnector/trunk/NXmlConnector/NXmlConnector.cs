using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NXmlConnector.Properties;

namespace NXmlConnector
{
    static class NXmlConnector
    {
        private const string EXPORT_DLL = "txmlconnector.dll";

        [DllImport(EXPORT_DLL, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr SendCommand(IntPtr ptr);

        [DllImport(EXPORT_DLL, CallingConvention = CallingConvention.Winapi)]
        private static extern bool FreeMemory(IntPtr ptr);

        [DllImport(EXPORT_DLL, CallingConvention = CallingConvention.Winapi)]
        private static extern bool SetCallback([MarshalAs(UnmanagedType.FunctionPtr)]TXmlConnectorCallBack callback);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate void TXmlConnectorCallBack(IntPtr ptr);
        
        
        private readonly static TXmlConnectorCallBack callback = new TXmlConnectorCallBack(CallBack);

        private readonly static Encoding utf8 = Encoding.UTF8;

        private static bool started = false;


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
                lock (callback)
                {
                    if (!started)
                    {
                        started = SetCallback(callback);
                        if (!started) throw new NXmlConnectorException(Resources.CanNotSetCallback);
                    }
                }
            }
        }

        private static void CallBack(IntPtr ptr)
        {
            var data = PtrToString(ptr);
            var ev = NewData;
            if (ev != null) ev(data);
        }

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
