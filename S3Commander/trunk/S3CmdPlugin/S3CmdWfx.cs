using System;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;
using System.Windows.Forms;

namespace S3CmdPlugin
{
	[TotalCommanderPlugin("S3CmdX")]
	[ResourcePluginIcon(typeof(S3CmdWfx), "S3CmdPlugin.Resources.PluginResources", "amazon")]
	public class S3CmdWfx : FileSystemPlugin
	{
		private IPathResolver pathResolver = new PluginRoot();


		public override string Name
		{
			get { return PluginResources.ProductName; }
		}

		public S3CmdWfx()
			: base()
		{
		}

		public override object FindFirst(string Path, ref FindData FindData)
		{
			try
			{
				var browsable = pathResolver.ResolvePath(Path) as IBrowsable;
				if (browsable != null)
				{
					browsable.Reset();
					if (browsable.MoveNext()) FindData = browsable.Current.FindData;
				}
				return browsable;
			}
			catch (Exception ex)
			{
				ShowError(ex);
				return null;
			}
		}

		public override bool FindNext(object Status, ref FindData FindData)
		{
			try
			{
				var browsable = Status as IBrowsable;
				if (browsable != null && browsable.MoveNext())
				{
					FindData = browsable.Current.FindData;
					return true;
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return false;
		}

		public override void FindClose(object Status)
		{
			try
			{
				var disposable = Status as IDisposable;
				if (disposable != null) disposable.Dispose();

				base.FindClose(Status);
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
		}


		public override ExecExitCode ExecuteFile(IntPtr hMainWin, ref string RemoteName, string Verb)
		{
			try
			{
				var file = pathResolver.ResolvePath(RemoteName) as IFile;
				if (file == null) return base.ExecuteFile(hMainWin, ref RemoteName, Verb);

				if (Verb == "open")
				{
					StatusInfo(RemoteName, OperationStatus.Start, OperationKind.MkDir);
					file.Open(hMainWin);
					StatusInfo(RemoteName, OperationStatus.End, OperationKind.MkDir);
					return ExecExitCode.OK;
				}
				if (Verb == "properties") return file.Properties(hMainWin);
				if (Verb.StartsWith("chmod ")) return file.ChMod(hMainWin, Verb.Substring(6));
				if (Verb.StartsWith("quote ")) return file.Quote(hMainWin, Verb.Substring(6));
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return ExecExitCode.Error;
		}

		private void ShowError(Exception ex)
		{
			MessageBox.Show(ex.ToString(), Name);
		}
		protected override ExecExitCode ExecuteCommand(IntPtr hMainWin, ref string RemoteName, string command)
		{
			return base.ExecuteCommand(hMainWin, ref RemoteName, command);
		}

		protected override ExecExitCode ShowFileInfo(IntPtr hMainWin, string RemoteName)
		{
			return base.ShowFileInfo(hMainWin, RemoteName);
		}
	}
}
