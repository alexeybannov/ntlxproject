using System.Collections.Generic;
using Tools.TotalCommanderT;

namespace S3CmdPlugin
{
	interface IDirectory : IEnumerator<IFindDataProvider>
	{
        bool Create(string directory);

        bool Remove();
	}
}
