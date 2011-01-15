namespace ASC.Common.Security
{
    public enum SecurityLevel
        : byte
    {
        None = 0x00,

        SysService = 0x05,

        Simple = 0x10,

        SessionTicket = 0x40,

        DelegateTicket = 0x80,

        WindowsSecurity = 0xFF
    }
}