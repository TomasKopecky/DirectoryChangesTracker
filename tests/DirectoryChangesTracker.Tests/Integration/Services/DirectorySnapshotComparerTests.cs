using DirectoryChangesTracker.Models;
using DirectoryChangesTracker.Services;

namespace DirectoryChangesTracker.Tests.Integration.Services
{
	/// <summary>
	/// Unit tests for the DirectorySnapshotComparer service which compares two snapshots and returns file and subdirectory changes.
	/// </summary>
	public class DirectorySnapshotComparerTests
	{
		/// <summary>
		/// Tests detection of new, deleted, and modified files based on LocalPath and Md5Hash.
		/// Also verifies subdirectory differences.
		/// </summary>
		[Fact]
		public void Compare_ShouldDetectFileAndSubdirectoryDifferences()
		{
			// Arrange
			var oldSnapshot = new DirectorySnapshot
			{
				LocalPath = "C:\\Test",
				Files = new HashSet<FileSnapshot>
				{
					new() { LocalPath = "C:\\Test\\old.txt", Name = "old", Md5Hash = "111", Version = 1 },
					new() { LocalPath = "C:\\Test\\mod.txt", Name = "mod", Md5Hash = "AAA", Version = 1 }
				},
				SubDirectories = new HashSet<string> { "C:\\Test\\OldSub" }
			};

			var newSnapshot = new DirectorySnapshot
			{
				LocalPath = "C:\\Test",
				Files = new HashSet<FileSnapshot>
				{
					new() { LocalPath = "C:\\Test\\mod.txt", Name = "mod", Md5Hash = "BBB", Version = 2 },
					new() { LocalPath = "C:\\Test\\new.txt", Name = "new", Md5Hash = "999", Version = 1 }
				},
				SubDirectories = new HashSet<string> { "C:\\Test\\NewSub" }
			};

			var comparer = new DirectorySnapshotComparer();

			// Act
			var result = comparer.Compare(oldSnapshot, newSnapshot);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Single(result.Value.NewCreatedFiles);
			Assert.Single(result.Value.DeletedFiles);
			Assert.Single(result.Value.ModifiedFiles);
			Assert.Single(result.Value.NewCreatedSubdirectories);
			Assert.Single(result.Value.DeletedSubdirectories);
			Assert.Equal("BBB", result.Value.ModifiedFiles.First().Md5Hash);
		}
	}
}
