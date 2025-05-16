namespace DirectoryChangesTracker.Models
{
	public class ScannedDirectoryResult
	{
		public required DirectorySnapshot DirectorySnapshot { get; set; }
		public IReadOnlyCollection<FileSnapshot> NewCreatedFiles { get; set; } = new HashSet<FileSnapshot>();
		public IReadOnlyCollection<FileSnapshot> ModifiedFiles { get; set; } = new HashSet<FileSnapshot>();
		public IReadOnlyCollection<FileSnapshot> DeletedFiles { get; set; } = new HashSet<FileSnapshot>();
	}
}
