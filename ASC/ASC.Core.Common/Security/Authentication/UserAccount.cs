#region usings

using System;
using ASC.Common.Security.Authentication;
using ASC.Core.Users;

#endregion

namespace ASC.Core.Security.Authentication
{
    [Serializable]
    public class UserAccount : AccountBase, IUserAccount
    {
        internal Credential Credential { get; private set; }

        internal UserAccount(Credential credential)
            : base(Guid.Empty, string.Empty)
        {
            if (credential == null) throw new ArgumentNullException("credential");
            Credential = credential;
        }

        internal UserAccount(IUserAccount userAccount)
            : base(userAccount.ID, userAccount.Name)
        {
            FirstName = userAccount.FirstName;
            LastName = userAccount.LastName;
            Title = userAccount.Title;
            Department = userAccount.Department;
            Tenant = userAccount.Tenant;
            Email = userAccount.Email;
        }

        internal UserAccount(UserInfo info, int tenant)
            : base(info.ID, UserFormatter.GetUserName(info))
        {
            FirstName = info.FirstName;
            LastName = info.LastName;
            Title = info.Title;
            Department = info.Department;
            Tenant = tenant;
            Email = info.Email;
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Title { get; private set; }

        public string Department { get; private set; }

        public int Tenant { get; private set; }

        public string Email { get; private set; }
    }
}