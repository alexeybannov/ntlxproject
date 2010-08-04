using System.Text;

namespace TotalCommander.Plugin.Wfx.Internal
{
	delegate int ProgressCallback(int pluginNumber, string sourceName, string targetName, int percentDone);

	delegate void LogCallback(int pluginNumber, int messageType, string logString);

	delegate bool RequestCallback(int pluginNumber, int requestType, string customTitle, string customText, StringBuilder defaultText, int maxLen);
}
