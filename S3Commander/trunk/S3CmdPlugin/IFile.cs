using System.Drawing;
using Tools.TotalCommanderT;

namespace S3CmdPlugin
{
	interface IFile
	{
		FindData FindData
		{
			get;
		}
		
		ExecExitCode Open(PluginContext context);

		ExecExitCode Properties(PluginContext context);

		ExecExitCode ChMod(string mod, PluginContext context);

		ExecExitCode Quote(string command, PluginContext context);

		FileSystemExitCode Move(string newName, bool overwrite, RemoteInfo info, PluginContext context);

		bool Delete(PluginContext context);

		IconExtractResult ExctractCustomIcon(IconExtractFlags ExtractFlags, ref Icon icon);
	}
}
