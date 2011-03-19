using System;
using System.Collections.Generic;

namespace ASC.Core
{
    public interface IAzService
    {
        IEnumerable<AzRecord> GetAces(int tenant, DateTime from);

        AzRecord SaveAce(int tenant, AzRecord r);

        void RemoveAce(int tenant, AzRecord r);
    }
}
