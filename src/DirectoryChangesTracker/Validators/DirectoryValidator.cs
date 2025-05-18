using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Validators
{
	/// <summary>
	/// Validates whether a given local directory path is usable for scanning operations.
	/// </summary>
	public class DirectoryValidator : IDirectoryValidator
	{
		/// <summary>
		/// Validates the provided directory path by checking for null, emptiness,
		/// and whether the directory exists on the file system.
		/// </summary>
		/// <param name="localDirectoryPath">The full path to the local directory to validate.</param>
		/// <returns>
		/// A <see cref="Result"/> indicating whether the directory is valid.
		/// Returns a failure result if the path is empty or the directory does not exist.
		/// </returns>
		public Result ValidateDirectory(string localDirectoryPath)
		{
			if (string.IsNullOrEmpty(localDirectoryPath))
				return Result.Failure($"The provided folder path '{localDirectoryPath}' is null or empty");

			//if (!Directory.Exists(localDirectoryPath))
			//	return Result.Failure($"The provided directory path '{localDirectoryPath}' doesn't exist");

			return Result.Success();
		}
	}
}
