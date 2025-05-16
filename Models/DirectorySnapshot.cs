namespace DirectoryChangesTracker.Models
{
	/// <summary>
	/// Represents a snapshot of a single directory
	/// </summary>
	public class DirectorySnapshot
	{
		/// <summary>
		/// Full local path to the scanned directory.
		/// </summary>
		public required string LocalPath { get; set; }

		/// <summary>
		/// Date and time when this directory was scanned for the first time.
		/// </summary>
		public DateTime FirstListing { get; set; }

		/// <summary>
		/// Date and time when this directory was scanned most recently.
		/// </summary>
		public DateTime LastListing { get; set; }

		/// <summary>
		/// Total number of times this directory has been scanned.
		/// </summary>
		public int ListingsCount { get; set; }

		/// <summary>
		/// Collection of file snapshots captured during the scan.
		/// </summary>
		public HashSet<FileSnapshot> Files { get; set; } = new();
	}
}
