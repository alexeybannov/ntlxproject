#region usings

using System;
using System.Net;
using System.Net.Sockets;

#endregion

namespace ASC.Net
{
    public sealed class TcpPortResolver
    {
        public const int MinPort = 51515;
        public const int MaxPort = 65534;

        public static int GetFreePort()
        {
            var random = new Random();
            const int TRY_COUNT = 100;
            int counter = 0;
            do
            {
                int port = random.Next(MaxPort - MinPort) + MinPort;
                if (IsFree(port)) return port;
                counter++;
                if (TRY_COUNT < counter) throw new InvalidOperationException("Can not find free port.");
            } while (true);
        }

        public static bool IsFree(int port)
        {
            try
            {
                using (var client = new TcpClient(new IPEndPoint(IPAddress.Any, port)))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}