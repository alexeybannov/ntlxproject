using System;

namespace TotalCommander.Plugin
{
	public class TotalCommanderPluginAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public TotalCommanderPluginAttribute(string name)
		{
			Name = name;
		}
	}
}
