using System;
using System.Collections.Generic;

namespace ASC.Core
{
    public interface IDbAzService
    {
        IEnumerable<AzRecord> GetAces(int tenant, DateTime from);

        void RemoveAce(int tenant, AzRecord r);
        
        void SaveAce(int tenant, AzRecord r);
    }
}
