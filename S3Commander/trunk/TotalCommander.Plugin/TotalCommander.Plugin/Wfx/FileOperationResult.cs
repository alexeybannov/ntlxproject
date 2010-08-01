
namespace TotalCommander.Plugin.Wfx
{
	public enum FileOperationResult
	{
		OK,
		Exists,
		NotFound,
		ReadError,
		WriteError,
		UserAbort,
		NotSupported,
		ExistsResumeAllowed,
        Default = NotSupported
	}
}
