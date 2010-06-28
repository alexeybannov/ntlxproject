using System;
using Tools.TotalCommanderT;

namespace S3CmdPlugin
{
	interface IFile
	{
		ExecExitCode Open(IntPtr mainWin);

		ExecExitCode Properties(IntPtr mainWin);

		ExecExitCode ChMod(IntPtr mainWin, string mod);

		ExecExitCode Quote(IntPtr mainWin, string command);
	}
}
