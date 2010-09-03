using System.Drawing;
using TotalCommander.Plugin.Wfx;

namespace S3CmdPlugin
{
	interface IFile
	{
		FindData FindData
		{
			get;
		}
		
		ExecuteResult Open(PluginContext context);

        ExecuteResult Properties(PluginContext context);

        ExecuteResult ChMod(string mod, PluginContext context);

        ExecuteResult Quote(string command, PluginContext context);

		FileOperationResult Move(string newName, bool overwrite, RemoteInfo info, PluginContext context);

		bool Delete(PluginContext context);

        CustomIconResult ExctractCustomIcon(CustomIconFlag ExtractFlags, ref Icon icon);
	}
}
