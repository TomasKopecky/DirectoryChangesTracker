using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Services
{
	public interface IDirectoryScanner
	{
		Task<Result<DirectoryScanner>> ScanDirectory(string localFolderPath);
	}
}
