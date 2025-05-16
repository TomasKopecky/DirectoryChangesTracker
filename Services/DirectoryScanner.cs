using DirectoryChangesTracker.Models;
using DirectoryChangesTracker.Validators;

namespace DirectoryChangesTracker.Services
{
	public class DirectoryScanner : IDirectoryScanner
	{
		private readonly IDirectoryValidator _directoryValidator;

		public DirectoryScanner(IDirectoryValidator directoryValidator)
		{
			_directoryValidator = directoryValidator;
		}

		public async Task<Result<DirectoryScanner>> ScanDirectory(string localFolderPath)
		{
			throw new NotImplementedException();
		}
	}
}
