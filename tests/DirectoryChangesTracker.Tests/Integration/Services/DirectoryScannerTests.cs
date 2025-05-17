using DirectoryChangesTracker.Models;
using DirectoryChangesTracker.Services;
using DirectoryChangesTracker.Validators;
using Moq;

namespace DirectoryChangesTracker.Tests.Integration.Services
{
	/// <summary>
	/// Unit tests for the DirectoryScanner service, which scans directories and builds a DirectorySnapshot.
	/// </summary>
	public class DirectoryScannerTests
	{
		/// <summary>
		/// Tests that a valid directory containing one file results in a successful scan.
		/// </summary>
		[Fact]
		public async Task ScanDirectory_ShouldReturnValidSnapshot_WithOneFile()
		{
			// Arrange
			var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(tempDir);
			var filePath = Path.Combine(tempDir, "test.txt");
			await File.WriteAllTextAsync(filePath, "Hello");

			var mockDirValidator = new Mock<IDirectoryValidator>();
			mockDirValidator.Setup(v => v.ValidateDirectory(tempDir)).Returns(Result.Success());

			var mockFileValidator = new Mock<IFileValidator>();
			mockFileValidator.Setup(f => f.ValidateFileSnapshot(filePath)).ReturnsAsync(Result<FileSnapshot>.Success(
				new FileSnapshot
				{
					LocalPath = filePath,
					Name = "test",
					Md5Hash = "FAKEHASH",
					Version = 1
				}));

			var scanner = new DirectoryScanner(mockDirValidator.Object, mockFileValidator.Object);

			// Act
			var result = await scanner.ScanDirectory(tempDir);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Single(result.Value.Files);

			// Cleanup
			File.Delete(filePath);
			Directory.Delete(tempDir);
		}
	}
}
