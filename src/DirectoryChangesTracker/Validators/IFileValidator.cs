using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Validators
{
	// <summary>
	/// Defines a contract for validating a file and generating its metadata snapshot.
	/// </summary>
	public interface IFileValidator
	{
		/// <summary>
		/// Validates a file at the specified path and creates a <see cref="FileSnapshot"/>
		/// if the file is accessible and valid.
		/// </summary>
		/// <param name="fileLocalPath">The full local path to the file.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a <see cref="FileSnapshot"/> if validation succeeds,
		/// or an error message if the file is invalid or inaccessible.
		/// </returns>
		Task<Result<FileSnapshot>> ValidateFileSnapshot(string fileLocalPath);
	}
}
