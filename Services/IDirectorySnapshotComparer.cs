using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Services
{
	/// <summary>
	/// Defines a contract for comparing two directory snapshots to identify file and subdirectory changes.
	/// </summary>
	public interface IDirectorySnapshotComparer
	{
		/// <summary>
		/// Compares two directory snapshots and returns the differences between them,
		/// including newly created, modified, and deleted files and subdirectories.
		/// </summary>
		/// <param name="oldSnapshot">The snapshot representing the previous state of the directory.</param>
		/// <param name="newSnapshot">The snapshot representing the current state of the directory.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a <see cref="ScannedDirectoryResult"/> if successful,
		/// or an error message if one of the snapshots is null or invalid.
		/// </returns>
		Result<ScannedDirectoryResult> Compare(DirectorySnapshot oldSnapshot, DirectorySnapshot newSnapshot);
	}
}
