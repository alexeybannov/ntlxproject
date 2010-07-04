using System;
using System.Collections.Generic;
using System.Text;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin.Sample.Wfx
{
	[TotalCommanderPlugin("sss")]
	public class HelloWorldWfxPlugin : TotalCommanderWfxPlugin
	{
		public override string Name
		{
			get { return "Hello, World!"; }
		}
	}
}
