using DirectoryChangesTracker.Models;
using System.Security.Cryptography;

namespace DirectoryChangesTracker.Validators
{
	/// <summary>
	/// Provides validation and metadata extraction for individual files.
	/// </summary>
	public class FileValidator : IFileValidator
	{
		/// <summary>
		/// Validates a file path and generates a <see cref="FileSnapshot"/> containing metadata
		/// such as the file name and MD5 hash, if the file is valid and readable.
		/// </summary>
		/// <param name="fileLocalPath">The full local path to the file.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a <see cref="FileSnapshot"/> on success,
		/// or an error message if the validation fails.
		/// </returns>
		public async Task<Result<FileSnapshot>> ValidateFileSnapshot(string fileLocalPath)
		{
			if (string.IsNullOrEmpty(fileLocalPath))
				return Result<FileSnapshot>.Failure($"The provided file path '{fileLocalPath}' is empty");

			else if (!File.Exists(fileLocalPath))
				return Result<FileSnapshot>.Failure($"The file on the provided file path '{fileLocalPath}' doesn't exist");

			try
			{
				byte[] fileBytesContent = await File.ReadAllBytesAsync(fileLocalPath);
				string? md5Hash = Convert.ToHexString(MD5.HashData(fileBytesContent));

				if (md5Hash == null)
					return Result<FileSnapshot>.Failure($"Error during calculating has of The file on the provided file path '{fileLocalPath}'");

				FileSnapshot fileSnapshot = new()
				{
					LocalPath = fileLocalPath,
					Md5Hash = md5Hash,
					Name = Path.GetFileNameWithoutExtension(fileLocalPath)
				};

				return Result<FileSnapshot>.Success(fileSnapshot);
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result<FileSnapshot>.Failure($"Exception caught during reading the file on the provided file path '{fileLocalPath}'");
			}
		}
	}
}
