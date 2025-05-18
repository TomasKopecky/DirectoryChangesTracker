namespace DirectoryChangesTracker.Models
{
	/// <summary>
	/// Represents a snapshot of a single file
	/// </summary>
	public class FileSnapshot
	{
		/// <summary>
		/// Full local path to the file.
		/// </summary>
		public required string LocalPath { get; set; }

		/// <summary>
		/// File name (without path).
		/// </summary>
		public required string Name { get; set; }

		/// <summary>
		/// MD5 hash of the file content at the time of scanning.
		/// </summary>
		public required string Md5Hash { get; set; }

		/// <summary>
		/// Version number that increments each time the file is modified.
		/// </summary>
		public int Version { get; set; } = 1;
	}
}
