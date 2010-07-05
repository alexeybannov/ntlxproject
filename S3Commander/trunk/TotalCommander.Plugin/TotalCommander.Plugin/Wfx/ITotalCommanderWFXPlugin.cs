
namespace TotalCommander.Plugin.Wfx
{
	public interface ITotalCommanderWfxPlugin
	{
		void SetDefaultParams(DefaultParam defaultParam);


		void Init(Progress progress, Logger logger, Request request);

		bool FindFirst(string path, FindData findData, out object enumerator);

		bool FindNext(object enumerator, FindData findData);

		void FindClose(object enumerator);


		ExecuteResult ExecuteFile(MainWindow mainWindow, string remoteName, string verb);

		FileOperationResult RenameMoveFile(string oldName, string newName, bool move, bool overWrite, RemoteInfo ri);
	}
}
