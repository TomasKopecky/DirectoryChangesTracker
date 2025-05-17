namespace DirectoryChangesTracker.Models
{
	/// <summary>
	/// Represents the result of a directory scan, including newly created,
	/// modified, and deleted files relative to a previous snapshot.
	/// </summary>
	public class ScannedDirectoryResult
	{
		/// <summary>
		/// The current snapshot of the scanned directory, including file metadata.
		/// </summary>
		public required DirectorySnapshot DirectorySnapshot { get; set; }

		/// <summary>
		/// Files that were detected as newly created since the last scan.
		/// </summary>
		public IReadOnlyCollection<FileSnapshot> NewCreatedFiles { get; set; } = new HashSet<FileSnapshot>();

		/// <summary>
		/// Files that were detected as modified since the last scan.
		/// </summary>
		public IReadOnlyCollection<FileSnapshot> ModifiedFiles { get; set; } = new HashSet<FileSnapshot>();

		/// <summary>
		/// Files that were present in the previous snapshot but no longer exist.
		/// </summary>
		public IReadOnlyCollection<FileSnapshot> DeletedFiles { get; set; } = new HashSet<FileSnapshot>();

		/// <summary>
		/// Subdirectories (paths) that were detected as newly created since the last scan.
		/// </summary>
		public IReadOnlyCollection<string> NewCreatedSubdirectories { get; set; } = new HashSet<string>();

		/// <summary>
		/// Subdirectories (paths) that were deleted since the last scan.
		/// </summary>
		public IReadOnlyCollection<string> DeletedSubdirectories { get; set; } = new HashSet<string>();
	}
}
