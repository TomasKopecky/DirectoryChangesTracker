using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Validators
{
	public interface IDirectoryValidator
	{
		Result ValidateDirectory(string localDirectoryPath);
	}
}
