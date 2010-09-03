using System;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// Marks a class as Total Commander plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class TotalCommanderPluginAttribute : Attribute
	{
	}
}
