using System;

namespace TotalCommander.Plugin
{
    [AttributeUsage(AttributeTargets.Class)]
	public class TotalCommanderPluginAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public TotalCommanderPluginAttribute(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			Name = name;
		}
	}
}
