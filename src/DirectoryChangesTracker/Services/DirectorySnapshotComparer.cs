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

			IncrementAndSetLastFileVersion(modifiedFiles, oldFiles, newFiles);

			scannedDirectoryResult.NewCreatedFiles = newCreatedFiles;
			scannedDirectoryResult.ModifiedFiles = modifiedFiles;
			scannedDirectoryResult.DeletedFiles = deletedFiles;
		}

		/// <summary>
		/// Increments the version number of modified files by comparing them with their previous versions.
		/// </summary>
		/// <param name="modifiedFiles">The set of files that have been detected as modified.</param>
		/// <param name="oldFiles">The set of file snapshots from the previous scan.</param>
		/// <param name="newFiles">The set of file snapshots from the current scan.</param>
		/// <remarks>
		/// For each file in <paramref name="newFiles"/> that is also in <paramref name="modifiedFiles"/> 
		/// and exists in <paramref name="oldFiles"/>, this method sets its version to the previous version + 1.
		/// </remarks>
		private void IncrementAndSetLastFileVersion(HashSet<FileSnapshot> modifiedFiles, HashSet<FileSnapshot> oldFiles, HashSet<FileSnapshot> newFiles)
		{
			if (oldFiles == null || newFiles == null || !oldFiles.Any() || !newFiles.Any())
				return;

			foreach (FileSnapshot newFile in newFiles)
			{
				if (oldFiles.Any(of => of.LocalPath == newFile.LocalPath))
				{
					int lastVersion = oldFiles.First(of => of.LocalPath == newFile.LocalPath).Version;
					newFile.Version = lastVersion;

					if (modifiedFiles.Any(mf => mf.LocalPath == newFile.LocalPath))
						newFile.Version++;
				}
			}
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
