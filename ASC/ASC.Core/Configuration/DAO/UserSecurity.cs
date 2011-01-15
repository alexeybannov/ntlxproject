using System;

namespace ASC.Core.Configuration.DAO
{
    public class UserSecurity
    {
        public Guid UserID
        {
            get;
            set;
        }

        public string PasswordHash
        {
            get;
            set;
        }

        public string PasswordHashSHA512
        {
            get;
            set;
        }

        public UserSecurity(Guid userId)
        {
            UserID = userId;
        }
    }
}
