namespace ASC.Common.Security.Authentication
{
    public enum AuthStrategy
        : byte
    {
        WindowsIntegrated = 1,

        LoginPassword = 2,

        SystemAccount = 3,

        CustomeService = 4
    }
}