using System.Collections.Generic;
using ASC.Core.Users;

namespace ASC.Core
{
    public interface IDbAzService
    {
        IEnumerable<AzRecord> GetAces(int tenant);

        void RemoveAce(int tenant, AzRecord r);
        
        void SaveAce(int tenant, AzRecord r);
    }
}
