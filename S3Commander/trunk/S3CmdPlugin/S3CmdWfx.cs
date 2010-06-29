using System;
using System.Windows.Forms;
using S3CmdPlugin.Resources;
using Tools.TotalCommanderT;
using System.Drawing;

namespace S3CmdPlugin
{
	[TotalCommanderPlugin("S3CmdX")]
	[ResourcePluginIcon(typeof(S3CmdWfx), "S3CmdPlugin.Resources.PluginResources", "amazon")]
	public class S3CmdWfx : FileSystemPlugin
	{
		private IPathResolver pathResolver;
		private PluginContext context;


		public override string Name
		{
			get { return PluginResources.ProductName; }
		}


		public S3CmdWfx()
			: base()
		{
			pathResolver = new PluginRoot();
			context = new PluginContext(this);
		}


		public override object FindFirst(string Path, ref FindData FindData)
		{
			try
			{
				var directory = pathResolver.ResolvePath(Path) as IDirectory;
				if (directory != null)
				{
					directory.Reset();
					if (directory.MoveNext())
					{
						FindData = directory.Current.FindData;
						return directory;
					}
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return null;
		}

		public override bool FindNext(object Status, ref FindData FindData)
		{
			try
			{
				var directory = Status as IDirectory;
				if (directory != null && directory.MoveNext())
				{
					FindData = directory.Current.FindData;
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


		public override bool MkDir(string Path)
		{
			try
			{
				if (!string.IsNullOrEmpty(Path))
				{
					Path = Path.TrimEnd('\\');
					var position = Path.LastIndexOf('\\') + 1;
					if (0 < position && position < Path.Length)
					{
						var directory = pathResolver.ResolvePath(Path.Substring(0, position)) as IDirectory;
						if (directory != null)
						{
							return directory.Create(Path.Substring(position), context);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return false;
		}

		public override ExecExitCode ExecuteFile(IntPtr hMainWin, ref string RemoteName, string Verb)
		{
			try
			{
				var file = pathResolver.ResolvePath(RemoteName);
				if (file != null)
				{
					context.SetHandle(hMainWin);
					var comparison = StringComparison.InvariantCultureIgnoreCase;
					if (Verb.StartsWith("open", comparison)) return file.Open(context);
					if (Verb.StartsWith("properties", comparison)) return file.Properties(context);
					if (Verb.StartsWith("chmod ", comparison)) return file.ChMod(Verb.Substring(6), context);
					if (Verb.StartsWith("quote ", comparison)) return file.Quote(Verb.Substring(6), context);
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return ExecExitCode.OK;
		}

		public override FileSystemExitCode RenMovFile(string OldName, string NewName, bool Move, bool OverWrite, RemoteInfo info)
		{
			try
			{
				var file = pathResolver.ResolvePath(OldName);
				if (file != null)
				{
					return file.Move(NewName, OverWrite, info, context);
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return FileSystemExitCode.NotSupported;
		}

		public override bool RemoveDir(string RemoteName)
		{
			return DeleteFile(RemoteName);
		}

		public override bool DeleteFile(string RemoteName)
		{
			try
			{
				var file = pathResolver.ResolvePath(RemoteName);
				if (file != null)
				{
					return file.Delete(context);
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return false;
		}


		public override IconExtractResult ExctractCustomIcon(ref string RemoteName, IconExtractFlags ExtractFlags, ref Icon TheIcon)
		{
			try
			{
				var file = pathResolver.ResolvePath(RemoteName);
				if (file != null)
				{
					return file.ExctractCustomIcon(ExtractFlags, ref TheIcon);
				}
			}
			catch (Exception ex)
			{
				ShowError(ex);
			}
			return IconExtractResult.UseDefault;
		}

		private void ShowError(Exception ex)
		{
#if DEBUG
			MessageBox.Show(ex.ToString(), Name);
#else
            MessageBox.Show(ex.Message, Name);
#endif
		}
	}
}
