using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Services
{
	/// <summary>
	/// Defines a contract for scanning a local directory
	/// and producing a structured snapshot of its contents.
	/// </summary>
	public interface IDirectoryScanner
	{
		/// <summary>
		/// Scans the specified directory and returns a snapshot
		/// containing validated files and subdirectories.
		/// </summary>
		/// <param name="localFolderPath">The full path to the directory to scan.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a <see cref="DirectorySnapshot"/> if successful,
		/// or an error message if the scan fails.
		/// </returns>
		Task<Result<DirectorySnapshot>> ScanDirectory(string localFolderPath);
	}
}
