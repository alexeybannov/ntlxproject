using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Ntlx.MSBuild.Tasks
{
	public class DllExport : Task
	{
		[Required]
		public string Assembly
		{
			get;
			set;
		}

		public string OutputType
		{
			get;
			set;
		}

		public string DebugSymbols
		{
			get;
			set;
		}

		public string SignAssembly
		{
			get;
			set;
		}

		public string KeyFile
		{
			get;
			set;
		}

		public string Platform
		{
			get;
			set;
		}


		public override bool Execute()
		{
			string ilFile = null;
			string resFile = null;
			string newIlFile = null;
			try
			{
				ilFile = ExecuteIldasm();
				resFile = Path.ChangeExtension(ilFile, "res");
				newIlFile = CreateExportTable(ilFile);
				if (newIlFile != null)
				{
					ExecuteIlasm(newIlFile, resFile);
				}
				return true;
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
				return false;
			}
			finally
			{
				Cleanup(ilFile, resFile, newIlFile);
			}
		}

		private string ExecuteIldasm()
		{
			var ildasm = Path.Combine(SystemPathProvider.MicrosoftSdkPath, "ildasm.exe");
			var ilFile = Path.GetTempFileName();
			var args = new StringBuilder()
				.AppendFormat("\"{0}\"", Assembly)
				.AppendFormat(" /out=\"{0}\"", ilFile)
				.AppendFormat(" /nobar")
				.AppendFormat(" /unicode")
				.ToString();

			Log.LogMessageFromText(string.Format("Ildasm commandline: {0} {1}", ildasm, args), MessageImportance.Normal);

			ExecProcess(ildasm, args);
			Log.LogMessageFromText("Success execute ildasm", MessageImportance.Normal);
			return ilFile;
		}

		private string CreateExportTable(string ilFile)
		{
			var newIlFile = Path.GetTempFileName();
			var exports = GetExportMethods(ilFile);
			if (exports.Count == 0)
			{
				Log.LogMessageFromText("Exports not found", MessageImportance.Normal);
				return null;
			}

			using (var reader = new StreamReader(ilFile, Encoding.Unicode))
			using (var writer = new StreamWriter(newIlFile, false, Encoding.Unicode))
			{
				var line = string.Empty;
				var isExport = false;
				var counter = 0;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith(".corflags "))
					{
						line = string.Format(".corflags 0x00000002\r\n.data VT_01 = int32[{0}]\r\n.vtfixup [{0}] int32 fromunmanaged at VT_01", exports.Count);
					}
					if (IsExport(line))
					{
						isExport = true;
					}
					if (line.Trim().StartsWith("IL_0000:"))
					{
						if (isExport)
						{
							line = string.Format(".vtentry 1:{0}\r\n.export [{0}] as {1}\r\n{2}", counter + 1, exports[counter], line);
							counter++;
						}
						isExport = false;
					}

					writer.WriteLine(line);
				}
			}
			return newIlFile;
		}

		private void ExecuteIlasm(string newIlFile, string resFile)
		{
			var ilasm = Path.Combine(SystemPathProvider.FrameworkPath, "ilasm.exe");
			var args = new StringBuilder(" /quite");
			if (OutputType == "Library") args.Append(" /dll");
			args.AppendFormat(" \"{0}\"", newIlFile);
			if (!string.IsNullOrEmpty(resFile) && File.Exists(resFile)) args.AppendFormat(" /resource:\"{0}\"", resFile);
			args.AppendFormat(" /output=\"{0}\"", Assembly);
			if (DebugSymbols == "true") args.Append(" /debug=IMPL");
			if (SignAssembly == "true") args.AppendFormat(" /key=\"{0}\"", KeyFile);
			if (Platform == "X64") args.Append(" /PE64");

			Log.LogMessageFromText(string.Format("Ilasm commandline: {0} {1}", ilasm, args), MessageImportance.Normal);

			ExecProcess(ilasm, args.ToString());
			Log.LogMessageFromText("Success execute ilasm", MessageImportance.Normal);
		}


		private int ExecProcess(string fileName, string args)
		{
			var pi = new ProcessStartInfo(fileName, args)
			{
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = Path.GetDirectoryName(BuildEngine.ProjectFileOfTaskNode)
			};
			using (var process = Process.Start(pi))
			{
				process.WaitForExit();
				var code = process.ExitCode;
				if (code != 0)
				{
					throw new Exception(string.Format("Process {0} exit with code {1}: {2}", fileName, code, args));
				}
				return process.ExitCode;
			}
		}

		private List<string> GetExportMethods(string ilFile)
		{
			var methods = new List<string>();
			using (var reader = new StreamReader(ilFile, Encoding.Unicode))
			{
				var line = string.Empty;
				var inMethod = false;
				var method = string.Empty;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.Trim().StartsWith(".method"))
					{
						inMethod = true;
					}
					if (inMethod && line.Contains("("))
					{
						if (string.IsNullOrEmpty(method))
						{
							method = line.Substring(0, line.IndexOf("("));
							method = method.Substring(method.LastIndexOf(" ") + 1);
						}
					}
					if (IsExport(line))
					{
						methods.Add(method);
					}
					if (line.Trim().StartsWith("IL_0000:") || line.Trim().StartsWith("} // end of method "))
					{
						inMethod = false;
						method = null;
					}
				}
			}
			return methods;
		}

		private bool IsExport(string line)
		{
			return line.Trim().StartsWith(".custom instance void ") && line.Contains("DllExportAttribute");
		}

		private void Cleanup(params string[] files)
		{
			foreach (var file in files)
			{
				try
				{
					if (!string.IsNullOrEmpty(file) && File.Exists(file))
					{
						File.Delete(file);
					}
				}
				catch { }
			}
		}
	}
}
