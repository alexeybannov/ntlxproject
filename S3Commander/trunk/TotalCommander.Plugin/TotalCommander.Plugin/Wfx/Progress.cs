using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class Progress
	{
		private int pluginNumber;

		private ProgressCallback progress;

		internal Progress(int pluginNumber, ProgressCallback progress)
		{
			if (progress == null) throw new ArgumentNullException("progress");

			this.pluginNumber = pluginNumber;
			this.progress = progress;
		}

		public int SetProgress(string source, string target, int percent)
		{
			return progress(pluginNumber, source, target, percent);
		}
	}
}
