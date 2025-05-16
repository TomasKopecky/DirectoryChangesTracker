namespace DirectoryChangesTracker.Models
{
	public class FileSnapshot
	{
		public required string LocalPath { get; set; }
		public required string Name { get; set; }
		public required string Md5Hash { get; set; }
		public int Version { get; set; }
	}
}
