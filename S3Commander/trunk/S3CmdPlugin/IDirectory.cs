using System.Collections.Generic;

namespace S3CmdPlugin
{
	interface IDirectory : IEnumerator<IFile>, IFile
	{
        bool Create(PluginContext context, string directory);
	}
}
