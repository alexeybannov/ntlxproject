using System;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// Indicates that the attributed method is exported from assembly.
    /// </summary>
    /// <remarks>
    /// Use by Ntlx.MSBuild.Tasks.DllExport msbuild task.
    /// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public class DllExportAttribute : Attribute
	{
	}
}
