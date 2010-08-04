using System;

namespace TotalCommander.Plugin
{
    /// <summary>
    /// Marks a class as Total Commander plugin and provides plugin name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class TotalCommanderPluginAttribute : Attribute
	{
        /// <summary>
        /// Plugin name.
        /// </summary>
		public string Name
		{
			get;
			private set;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalCommanderPluginAttribute"/> class.
        /// </summary>
        /// <param name="name">Plugin name.</param>
		public TotalCommanderPluginAttribute(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			Name = name;
		}
	}
}
