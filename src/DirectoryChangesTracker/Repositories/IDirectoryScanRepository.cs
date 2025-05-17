using DirectoryChangesTracker.Models;

namespace DirectoryChangesTracker.Repositories
{
	/// <summary>
	/// Defines a contract for saving and loading scanned directory results to and from a persistent storage.
	/// </summary>
	public interface IDirectoryScanRepository
	{
		/// <summary>
		/// Loads all previously saved directory scan results from storage.
		/// </summary>
		/// <returns>A <see cref="Result{T}"/> containing the scanned results or an error message.</returns>
		Task<Result<IReadOnlyCollection<ScannedDirectoryResult>>> LoadAllAsync();

		/// <summary>
		/// Saves all provided scanned directory results to storage.
		/// </summary>
		/// <param name="results">A collection of scanned results to be saved.</param>
		/// <returns>A <see cref="Result"/> indicating success or failure.</returns>
		Task<Result> SaveAllAsync(IEnumerable<ScannedDirectoryResult> results);
	}
}
