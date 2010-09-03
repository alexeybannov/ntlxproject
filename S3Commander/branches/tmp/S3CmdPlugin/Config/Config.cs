using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.TotalCommanderT;

namespace S3CmdPlugin.Config
{
	class ConfigRoot : FileBase
	{
		public ConfigRoot()
		{
			findData.FileName = "Settings";
		}

		public override Tools.TotalCommanderT.ExecExitCode Open(PluginContext context)
		{
			using (var form = new ConfigForm())
			{
				form.ShowDialog();
			}
			return ExecExitCode.OK;
		}

		public override Tools.TotalCommanderT.ExecExitCode Properties(PluginContext context)
		{
			return Open(context);
		}
	}
}
