
namespace TotalCommander.Plugin.Wfx
{
	public enum FileOperationResult
	{
		OK = 0,
		Exists = 1,
		NotFound = 2,
		ReadError = 3,
		WriteError = 4,
		UserAbort = 5,
		NotSupported = 6,
		ExistsResumeAllowed = 7
	}
}
