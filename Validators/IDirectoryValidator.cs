using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Validators
{
	/// <summary>
	/// Validates whether a given directory path is suitable for scanning.
	/// </summary>
	public interface IDirectoryValidator
	{
		/// <summary>
		/// Validates the provided directory path and ensures it exists.
		/// </summary>
		/// <param name="localDirectoryPath">The full local path to the directory.</param>
		/// <returns>
		/// A <see cref="Result"/> indicating whether the directory is valid or not.
		/// </returns>
		Result ValidateDirectory(string localDirectoryPath);
	}
}
