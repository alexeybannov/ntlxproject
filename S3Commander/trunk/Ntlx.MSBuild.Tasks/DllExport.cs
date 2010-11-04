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
            using (var reader = new StreamReader(ilFile, Encoding.Unicode))
            using (var writer = new StreamWriter(newIlFile, false, Encoding.Unicode))
            {
                var line = string.Empty;
                var isExport = false;
                var counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith(".custom instance void ") && line.Contains("DllExportAttribute"))
                    {
                        isExport = true;
                    }
                    if (line.Trim().StartsWith("IL_0000:") && isExport)
                    {
                        line = string.Format(".export [{0}]", ++counter) + Environment.NewLine + line;
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
