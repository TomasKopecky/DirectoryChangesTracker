using DirectoryChangesTracker.Models;
using DirectoryChangesTracker.Validators;

namespace DirectoryChangesTracker.Services
{
	/// <summary>
	/// Provides functionality to scan a directory and retrieve information
	/// about its files and subdirectories.
	/// </summary>
	public class DirectoryScanner : IDirectoryScanner
	{
		private readonly IDirectoryValidator _directoryValidator;
		private readonly IFileValidator _fileValidator;

		/// <summary>
		/// Initializes a new instance of the <see cref="DirectoryScanner"/> class.
		/// </summary>
		/// <param name="directoryValidator">Service used to validate directory paths.</param>
		/// <param name="fileValidator">Service used to validate individual files.</param>

		public DirectoryScanner(IDirectoryValidator directoryValidator, IFileValidator fileValidator)
		{
			_directoryValidator = directoryValidator;
			_fileValidator = fileValidator;
		}

		/// <summary>
		/// Scans the given directory, validates its contents, and returns a snapshot
		/// of files and subdirectories found.
		/// </summary>
		/// <param name="localFolderPath">The full path to the directory to scan.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a <see cref="DirectorySnapshot"/> if successful,
		/// or an error message if the directory or its contents cannot be processed.
		/// </returns>
		public async Task<Result<DirectorySnapshot>> ScanDirectory(string localFolderPath)
		{
			Result validationResult = _directoryValidator.ValidateDirectory(localFolderPath);

			if (!validationResult.IsSuccess)
				return Result<DirectorySnapshot>.Failure(validationResult.ErrorMessage ?? $"Provided directory '{localFolderPath}' validation error occured");

			Result<HashSet<FileSnapshot>>? directoryFiles = await GetDirectoryFiles(localFolderPath);

			if (!directoryFiles.IsSuccess)
				return Result<DirectorySnapshot>.Failure(directoryFiles.ErrorMessage ?? $"Error during getting the files in the provided directory '{localFolderPath}'");

			Result<HashSet<string>> subDirectories = GetSubDirectories(localFolderPath);

			if (!subDirectories.IsSuccess)
				return Result<DirectorySnapshot>.Failure(subDirectories.ErrorMessage ?? $"Error during getting the sub directories of the provided directory '{localFolderPath}'");

			DirectorySnapshot directorySnapshot = new() { LocalPath = localFolderPath };

			directorySnapshot.SubDirectories = subDirectories.Value;
			directorySnapshot.Files = directoryFiles.Value;
			directorySnapshot.LastListing = DateTime.Now;

			return Result<DirectorySnapshot>.Success(directorySnapshot);
		}

		/// <summary>
		/// Retrieves and validates all files in the given directory.
		/// </summary>
		/// <param name="localFolderPath">The directory to search for files.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing a set of valid <see cref="FileSnapshot"/> instances,
		/// or an error message if file processing fails.
		/// </returns>
		public async Task<Result<HashSet<FileSnapshot>>> GetDirectoryFiles(string localFolderPath)
		{
			string[] files;
			try
			{
				files = Directory.GetFiles(localFolderPath);
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result<HashSet<FileSnapshot>>.Failure($"Error during obtaining files from the folder on path '{localFolderPath}'");
			}

			HashSet<FileSnapshot> resultFileSnaphots = new();

				foreach (string filePath in files)
				{
					Result<FileSnapshot>? validationResult = await _fileValidator.ValidateFileSnapshot(filePath);

					if (!validationResult.IsSuccess)
					{
						//TODO: log the unsucessfull validation
						//continue;
						return Result<HashSet<FileSnapshot>>.Failure(validationResult.ErrorMessage ?? $"Validation of the file on file path '{filePath}' failed.");
					}

					resultFileSnaphots.Add(validationResult.Value);
				}

			return Result<HashSet<FileSnapshot>>.Success(resultFileSnaphots);
		}

		/// <summary>
		/// Retrieves the names of all immediate subdirectories within a given directory.
		/// </summary>
		/// <param name="localDirectoryPath">The path of the parent directory.</param>
		/// <returns>
		/// A <see cref="Result{T}"/> containing the list of subdirectory paths,
		/// or an error message if enumeration fails.
		/// </returns>
		public Result<HashSet<string>> GetSubDirectories(string localDirectoryPath)
		{
			try
			{
				HashSet<string> subDirectories = Directory.GetDirectories(localDirectoryPath).ToHashSet();

				return Result<HashSet<string>>.Success(subDirectories);
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result<HashSet<string>>.Failure($"Error during getting sub folders for a directory '{localDirectoryPath}'");
			}
		}
	}
}
