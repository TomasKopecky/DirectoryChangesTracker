using DirectoryChangesTracker.Models;
using DirectoryChangesTracker.Repositories;
using DirectoryChangesTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryChangesTracker.Controllers
{
	/// <summary>
	/// Controller responsible for scanning a local directory, comparing results,
	/// and displaying file and directory changes to the user.
	/// </summary>
	public class ScanDirectoryController : Controller
	{
		private IDirectoryScanRepository _directoryScanRepository;
		private IDirectoryScanner _directoryScanner;
		private IDirectorySnapshotComparer _directorySnapshotComparer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScanDirectoryController"/>.
		/// </summary>
		/// <param name="directoryScanRepository">Repository used to load and save scan results.</param>
		/// <param name="directoryScanner">Service used to scan the current state of the directory.</param>
		/// <param name="directorySnapshotComparer">Service used to compare old and new directory snapshots.</param>
		public ScanDirectoryController(IDirectoryScanRepository directoryScanRepository, IDirectoryScanner directoryScanner, IDirectorySnapshotComparer directorySnapshotComparer)
		{
			_directoryScanRepository = directoryScanRepository;
			_directoryScanner = directoryScanner;
			_directorySnapshotComparer = directorySnapshotComparer;
		}

		/// <summary>
		/// Displays the initial directory scan form.
		/// </summary>
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Handles POST requests to scan a directory, compare it to the previous state,
		/// and display the result including changes in files and subdirectories.
		/// </summary>
		/// <param name="localDirectoryPath">The local directory path entered by the user.</param>
		[HttpPost]
		public async Task<IActionResult> ScanDirectory(DirectorySnapshot directorySnapshot)
		{
			if (directorySnapshot == null || string.IsNullOrEmpty(directorySnapshot.LocalPath))
			{
				ModelState.AddModelError(nameof(directorySnapshot.LocalPath), "The local directory path is required.");
				return View();
			}

			Result<IReadOnlyCollection<ScannedDirectoryResult>> loadedJsonFileResults = await _directoryScanRepository.LoadAllAsync();
			if (!SuccessOrAddModelError(loadedJsonFileResults, "Failed to load the saved scan history."))
				return View(null);

			string localDirectoryPath = directorySnapshot.LocalPath;

			if (!Directory.Exists(localDirectoryPath))
			{
				await DeleteAndSaveNonExistingDirectory(loadedJsonFileResults.Value, directorySnapshot);
				ModelState.AddModelError(string.Empty, $"The directory on path '{localDirectoryPath}' doesn't exist.");
				return View(null);
			}

			Result<DirectorySnapshot> directoryScannerCurrentResult = await _directoryScanner.ScanDirectory(localDirectoryPath);
			if (!SuccessOrAddModelError(directoryScannerCurrentResult, $"Failed to scan the directory on path '{localDirectoryPath}'"))
				return View(null);

			ScannedDirectoryResult directoryScannerPreviousResult = new ScannedDirectoryResult() { DirectorySnapshot = directorySnapshot };

			if (loadedJsonFileResults.Value.Any(fr => fr.DirectorySnapshot.LocalPath == localDirectoryPath))
				directoryScannerPreviousResult = loadedJsonFileResults.Value.First(fr => fr.DirectorySnapshot.LocalPath == localDirectoryPath);

			Result<ScannedDirectoryResult> snapshotComparerResult = _directorySnapshotComparer.Compare(directoryScannerPreviousResult.DirectorySnapshot, directoryScannerCurrentResult.Value);
			if (!SuccessOrAddModelError(snapshotComparerResult, $"Failed to compare the current and previous snapshots of the directory on path '{localDirectoryPath}'"))
				return View(null);

			Result updateAndSaveScanResult = await UpdateAndSaveCompareResult(loadedJsonFileResults.Value, snapshotComparerResult.Value);
			if (!SuccessOrAddModelError(updateAndSaveScanResult, "Failed to update and save the current directory snapshot result"))
				return View(null);

			return View(snapshotComparerResult.Value);
		}

		/// <summary>
		/// Adds a model error if the result indicates failure.
		/// </summary>
		/// <param name="actionResult">The result to check.</param>
		/// <param name="errorMessage">Fallback message if the result does not contain one.</param>
		private bool SuccessOrAddModelError(Result actionResult, string errorMessage)
		{
			if (!actionResult.IsSuccess)
			{
				ModelState.AddModelError(string.Empty, actionResult.ErrorMessage ?? errorMessage);
				return false;
			}

			return true;
		}


		/// <summary>
		/// Adds a model error if the result indicates failure.
		/// </summary>
		/// <typeparam name="T">The type of the result's value.</typeparam>
		/// <param name="actionResult">The result to check.</param>
		/// <param name="errorMessage">Fallback message if the result does not contain one.</param>
		private bool SuccessOrAddModelError<T>(Result<T> actionResult, string errorMessage)
		{
			if (!actionResult.IsSuccess)
			{
				ModelState.AddModelError(string.Empty, actionResult.ErrorMessage ?? errorMessage);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Updates the scanned results with the newly scanned result and persists the updated collection to storage.
		/// </summary>
		/// <param name="scannedDirectoryResults">The list of previously scanned directory results.</param>
		/// <param name="newScannedDirectoryResult">The newly generated scanned result.</param>
		private async Task<Result> UpdateAndSaveCompareResult(IReadOnlyCollection<ScannedDirectoryResult> scannedDirectoryResults, ScannedDirectoryResult newScannedDirectoryResult)
		{
			List<ScannedDirectoryResult> scannedDirectoryModifiedResults = scannedDirectoryResults.ToList();

			try
			{
				ScannedDirectoryResult matchingScannedDirectoryResult = new() { DirectorySnapshot = newScannedDirectoryResult.DirectorySnapshot };

				if (scannedDirectoryResults.Any(dr => dr.DirectorySnapshot.LocalPath == newScannedDirectoryResult.DirectorySnapshot.LocalPath))
					matchingScannedDirectoryResult = scannedDirectoryResults.First(dr => dr.DirectorySnapshot.LocalPath == newScannedDirectoryResult.DirectorySnapshot.LocalPath);

				newScannedDirectoryResult.DirectorySnapshot.FirstListing = matchingScannedDirectoryResult.DirectorySnapshot.FirstListing;
				newScannedDirectoryResult.DirectorySnapshot.ListingsCount = matchingScannedDirectoryResult.DirectorySnapshot.ListingsCount + 1;

				matchingScannedDirectoryResult = newScannedDirectoryResult;
				matchingScannedDirectoryResult.IsNew = false;

				// if the result for the specific path doesn't exist yet - create it
				if (!scannedDirectoryModifiedResults.Any(mr => mr.DirectorySnapshot.LocalPath == matchingScannedDirectoryResult.DirectorySnapshot.LocalPath))
				{
					matchingScannedDirectoryResult.IsNew = true;
					scannedDirectoryModifiedResults.Add(matchingScannedDirectoryResult);
				}

				// if it already exist - then swap the old result with the new one
				else
				{
					int resultToSwapIndex = scannedDirectoryModifiedResults.FindIndex(mr => mr.DirectorySnapshot.LocalPath == matchingScannedDirectoryResult.DirectorySnapshot.LocalPath);
					scannedDirectoryModifiedResults[resultToSwapIndex] = matchingScannedDirectoryResult;
				}

				await _directoryScanRepository.SaveAllAsync(scannedDirectoryModifiedResults);

				return Result.Success();
			}
			catch (Exception ex)
			{
				//TODO: log the exception
				return Result.Failure("Error during saving the new directory scan result");
			}
		}

		/// <summary>
		/// Deletes the possible already stored result in the storage for a directory that doesn't already exist
		/// </summary>
		/// <param name="scannedDirectoryResults">The list of previously scanned directory results.</param>
		/// <param name="directorySnapshot">The newly created directory snapshot.</param>
		private async Task<Result> DeleteAndSaveNonExistingDirectory(IReadOnlyCollection<ScannedDirectoryResult> scannedDirectoryResults, DirectorySnapshot directorySnapshot)
		{
			if (!scannedDirectoryResults.Any(dr => dr.DirectorySnapshot.LocalPath == directorySnapshot.LocalPath))
				return Result.Success();

			List<ScannedDirectoryResult> scannedDirectoryResultsList = scannedDirectoryResults.ToList();

			try
			{
				string errorMessage = "Failed to delete the currently non-existing directory from the scan history.";
				int matchingDirectorySnapshotIndex = scannedDirectoryResultsList.FindIndex(dr => dr.DirectorySnapshot.LocalPath == directorySnapshot.LocalPath);

				if (matchingDirectorySnapshotIndex == -1)
					return Result.Success();

				scannedDirectoryResultsList.RemoveAt(matchingDirectorySnapshotIndex);

				Result savedJsonFileResults = await _directoryScanRepository.SaveAllAsync(scannedDirectoryResultsList);

				if (!SuccessOrAddModelError(savedJsonFileResults, errorMessage))
					return Result.Failure(errorMessage);

				return Result.Success();
			}
			catch (Exception ex)
			{
				return Result.Failure($"Failed to delete the already non-existing directory '{directorySnapshot.LocalPath}' and save it to the scan history.");
			}
		}
	}
}
