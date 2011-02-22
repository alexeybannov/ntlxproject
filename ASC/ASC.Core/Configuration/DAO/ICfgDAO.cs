using System;
using System.Collections.Generic;
using ASC.Common.Security.Authentication;

namespace ASC.Core.Configuration.DAO
{
    public interface ICfgDAO
    {
        void SaveSettings(string key, byte[] data);
        
        byte[] GetSettings(string key);


        IUserAccount GetAccount(Credential credential);

        IEnumerable<Guid> GetAccountRoles(Guid accountId);
    }
}
