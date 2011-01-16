using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASC.Core.Common.Contract
{
    public interface IUserService
    {
        IEnumerable<User> GetUsers(int tenant);


    }
}
