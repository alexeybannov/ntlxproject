using System;
using Tools.TotalCommanderT;

namespace S3CmdPlugin
{
	interface IFile
	{
        ExecExitCode Open(MainWindow mainWindow);

        ExecExitCode Properties(MainWindow mainWindow);

		ExecExitCode ChMod(MainWindow mainWindow, string mod);

        ExecExitCode Quote(MainWindow mainWindow, string command);

        bool Delete();
	}
}
