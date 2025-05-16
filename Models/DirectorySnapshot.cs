namespace DirectoryChangesTracker.Models
{
	public class DirectorySnapshot
	{
		public required string LocalPath { get; set; }
		public DateTime FirstListing { get; set; }
		public DateTime LastListing { get; set; }
		public int ListingsCount { get; set; }

		HashSet<FileSnapshot> currentFiles = new();
	}
}
