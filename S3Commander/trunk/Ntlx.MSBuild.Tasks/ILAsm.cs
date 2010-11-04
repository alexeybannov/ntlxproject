using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using System.Collections;
using Microsoft.Build.Framework;

namespace Ntlx.MSBuild.Tasks
{
    public class ILAsm : ToolTask
    {
        /// <summary>
        /// Disable inheriting from System.Object by default.
        /// </summary>
        public bool NoAutoInherit
        {
            get;
            set;
        }

        public string OutputType
        {
            get;
            set;
        }

        public bool CreatePdb
        {
            get;
            set;
        }

        public bool Optimize
        {
            get;
            set;
        }

        public string Debug
        {
            get;
            set;
        }

        public bool Fold
        {
            get;
            set;
        }

        public string Resource
        {
            get;
            set;
        }

        [Output]
        public string Output
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public string Include
        {
            get;
            set;
        }

        protected override string ToolName
        {
            get { return "ILAsm.exe"; }
        }

        protected override string GenerateFullPathToTool()
        {
            return ToolLocationHelper.GetPathToDotNetFrameworkFile(ToolName, TargetDotNetFrameworkVersion.VersionLatest);
        }

        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();
            /*
            // We don't need the tool's logo information shown
            builder.AppendSwitch("/nologo");

            string targetType = Bag["TargetType"] as string;
            // Be explicit with our switches
            if (targetType != null)
            {
                if (String.Compare(targetType, "DLL", true) == 0)
                {
                    builder.AppendSwitch("/DLL");
                }
                else if (String.Compare(targetType, "EXE", true) == 0)
                {
                    builder.AppendSwitch("/EXE");
                }
                else
                {
                    Log.LogWarning("Invalid TargetType (valid values are DLL and EXE) specified: {0}", targetType);
                }                
            }

            // Add the filename that we want the tool to process
            builder.AppendFileNameIfNotNull(Bag["Source"] as ITaskItem);

            // Log a High importance message stating the file that we are assembling
            Log.LogMessage(MessageImportance.High, "Assembling {0}", Bag["Source"]);
            */
            // We have all of our switches added, return the commandline as a string
            return builder.ToString();
        }
    }
}
