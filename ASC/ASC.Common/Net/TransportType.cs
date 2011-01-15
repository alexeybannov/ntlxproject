namespace ASC.Net
{
    public enum TransportType : byte
    {
        Tcp = 0x01,
        Http = 0x02,
        Ipc = 0x03,
        Pipe = 0x04,
        Udp = 0x05,
    }
}