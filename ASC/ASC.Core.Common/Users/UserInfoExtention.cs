namespace ASC.Core.Users
{
    public static class UserInfoExtention
    {
        public static string DisplayName(this UserInfo ui)
        {
            return UserFormatter.GetUserName(ui);
        }
    }
}