using DirectoryChangesTracker.Models;
using System.Text.Json;

namespace DirectoryChangesTracker.Repositories
{
	/// <summary>
	/// A JSON-based implementation of <see cref="IDirectoryScanRepository"/> that saves and loads scanned directory results to a local file.
	/// </summary>
	public class JsonDirectoryScanRepository : IDirectoryScanRepository
	{
		private readonly string _jsonOutputFilePath;

		/// <summary>
		/// Initializes a new instance of the repository with the specified file path for JSON persistence.
		/// </summary>
		/// <param name="jsonOutputFilePath">The file path where scan results will be saved and loaded.</param>
		public JsonDirectoryScanRepository(string jsonOutputFilePath)
		{
			_jsonOutputFilePath = jsonOutputFilePath;
		}

		/// <inheritdoc />
		public async Task<Result<IReadOnlyCollection<ScannedDirectoryResult>>> LoadAllAsync()
		{
			try
			{
				if (!File.Exists(_jsonOutputFilePath))
				{
					try
					{
						// Create an empty file with an empty collection
						await File.WriteAllTextAsync(_jsonOutputFilePath, "[]");
					}
					catch (Exception ex)
					{
						//TODO: log the exception
						return Result<IReadOnlyCollection<ScannedDirectoryResult>>.Failure("Error when trying to create the output JSON storage file");
					}
				}

				string json = await File.ReadAllTextAsync(_jsonOutputFilePath);
				List<ScannedDirectoryResult>? result = JsonSerializer.Deserialize<List<ScannedDirectoryResult>>(json);

				return Result<IReadOnlyCollection<ScannedDirectoryResult>>.Success(result ?? new List<ScannedDirectoryResult>());
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result<IReadOnlyCollection<ScannedDirectoryResult>>.Failure("Error when trying to load the saved data from the JSON file");
			}
		}

		/// <inheritdoc />
		public async Task<Result> SaveAllAsync(IEnumerable<ScannedDirectoryResult> results)
		{
			try
			{
				string json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
				await File.WriteAllTextAsync(_jsonOutputFilePath, json);
				return Result.Success();
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result.Failure("Error when trying to save the data to the JSON file");
			}
		}
	}
}
