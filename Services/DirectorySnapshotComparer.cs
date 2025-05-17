using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Services
{
	/// <summary>
	/// Implements logic to compare two <see cref="DirectorySnapshot"/> instances and
	/// determine the differences between their files and subdirectories.
	/// </summary>
	public class DirectorySnapshotComparer : IDirectorySnapshotComparer
	{
		/// <inheritdoc />
		public Result<ScannedDirectoryResult> Compare(DirectorySnapshot oldSnapshot, DirectorySnapshot newSnapshot)
		{
			if (oldSnapshot == null || newSnapshot == null)
				return Result<ScannedDirectoryResult>.Failure("One of the provided directory snapshots is null");

			ScannedDirectoryResult scannedDirectoryResult = CalculateDifferences(oldSnapshot, newSnapshot);

			return Result<ScannedDirectoryResult>.Success(scannedDirectoryResult);
		}

		/// <summary>
		/// Calculates the differences between two directory snapshots, including added,
		/// modified, and deleted files and subdirectories.
		/// </summary>
		/// <param name="oldSnapshot">The original snapshot of the directory.</param>
		/// <param name="newSnapshot">The latest snapshot of the directory.</param>
		/// <returns>
		/// A <see cref="ScannedDirectoryResult"/> containing lists of changed files and subdirectories.
		/// </returns>

		private ScannedDirectoryResult CalculateDifferences(DirectorySnapshot oldSnapshot, DirectorySnapshot newSnapshot)
		{
			ScannedDirectoryResult scannedDirectoryResult = new() { DirectorySnapshot = newSnapshot };

			if (newSnapshot.LocalPath != oldSnapshot.LocalPath)
				return scannedDirectoryResult;

			CompareFiles(oldSnapshot, newSnapshot, scannedDirectoryResult);
			CompareSubdirectories(oldSnapshot, newSnapshot, scannedDirectoryResult);

			return scannedDirectoryResult;
		}

		/// <summary>
		/// Compares the file sets of two directory snapshots and populates the result with added, modified, and deleted files.
		/// </summary>
		/// <param name="oldSnapshot">The original snapshot of the directory.</param>
		/// <param name="newSnapshot">The latest snapshot of the directory.</param>
		/// <param name="scannedDirectoryResult">The result object to populate with file changes.</param>

		private void CompareFiles(DirectorySnapshot oldSnapshot, DirectorySnapshot newSnapshot, ScannedDirectoryResult scannedDirectoryResult)
		{
			HashSet<FileSnapshot> newFiles = newSnapshot.Files;
			HashSet<FileSnapshot> oldFiles = oldSnapshot.Files;

			// get the newly created files
			HashSet<FileSnapshot> newCreatedFiles = newFiles
				.Where(nf => oldFiles.All(of => of.LocalPath != nf.LocalPath))
				.ToHashSet();

			// get the deleted files
			HashSet<FileSnapshot> deletedFiles = oldFiles
				.Where(of => newFiles.All(nf => nf.LocalPath != of.LocalPath))
				.ToHashSet();

			// get the modified files
			HashSet<FileSnapshot> modifiedFiles = newFiles
				.Where(nf => oldFiles.Any(of => of.LocalPath == nf.LocalPath && of.Md5Hash != nf.Md5Hash))
				.ToHashSet();

			// set the version 1 for the new created files
			foreach (var newFile in newCreatedFiles)
				newFile.Version = 1;

			// increment the version for the modified files
			foreach (var modifiedFile in modifiedFiles)
				modifiedFile.Version += 1;

			scannedDirectoryResult.NewCreatedFiles = newCreatedFiles;
			scannedDirectoryResult.ModifiedFiles = modifiedFiles;
			scannedDirectoryResult.DeletedFiles = deletedFiles;
		}

		/// <summary>
		/// Compares the subdirectory sets of two directory snapshots and populates the result with added and deleted subdirectories.
		/// </summary>
		/// <param name="oldSnapshot">The original snapshot of the directory.</param>
		/// <param name="newSnapshot">The latest snapshot of the directory.</param>
		/// <param name="scannedDirectoryResult">The result object to populate with subdirectory changes.</param>

		private void CompareSubdirectories(DirectorySnapshot oldSnapshot, DirectorySnapshot newSnapshot, ScannedDirectoryResult scannedDirectoryResult)
		{
			HashSet<string> newSubdirectories = newSnapshot.SubDirectories;
			HashSet<string> oldSubdirectories = oldSnapshot.SubDirectories;

			HashSet<string> newCreatedSubdirectories = newSubdirectories
				.Where(ns => oldSubdirectories.All(os => os != ns))
				.ToHashSet();

			HashSet<string> deletedSubdirectories = oldSubdirectories
				.Where(os => newSubdirectories.All(ns => ns != os))
				.ToHashSet();

			scannedDirectoryResult.NewCreatedSubdirectories = newCreatedSubdirectories;
			scannedDirectoryResult.DeletedSubdirectories = deletedSubdirectories;
		}
	}
}
