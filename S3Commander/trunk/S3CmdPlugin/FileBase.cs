using System.Drawing;
using Tools.TotalCommanderT;

namespace S3CmdPlugin
{
	class FileBase : IFile
	{
		protected FindData findData;


		public FindData FindData
		{
			get { return findData; }
		}

		public virtual ExecExitCode Open(PluginContext context)
		{
			return ExecExitCode.Error;
		}

		public virtual ExecExitCode Properties(PluginContext context)
		{
            return ExecExitCode.Error;
		}

		public virtual ExecExitCode ChMod(string mod, PluginContext context)
		{
            return ExecExitCode.Error;
		}

		public virtual ExecExitCode Quote(string command, PluginContext context)
		{
            return ExecExitCode.Error;
		}

		public virtual FileSystemExitCode Move(string newName, bool overwrite, RemoteInfo info, PluginContext context)
		{
			return FileSystemExitCode.NotSupported;
		}

		public virtual bool Delete(PluginContext context)
		{
			return false;
		}

		public virtual IconExtractResult ExctractCustomIcon(IconExtractFlags ExtractFlags, ref Icon icon)
		{
			return IconExtractResult.UseDefault;
		}

		public override string ToString()
		{
			return findData.FileName;
		}
	}
}
