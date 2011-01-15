using ASC.Core.Users;

namespace ASC.Core.Host
{
    public interface IAscWSEntryPoint
    {
        User[] GetUsers();
    }

    public class User
    {
        public User(UserInfo userInfo)
        {
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
        }

        public string FirstName;

        public string LastName;
    }
}
