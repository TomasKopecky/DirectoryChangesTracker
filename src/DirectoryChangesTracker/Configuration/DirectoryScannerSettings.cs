namespace DirectoryChangesTracker.Configuration
{
	public class DirectoryScannerSettings
	{
		/// <summary>
		/// Whether to store the scan result in the user's temporary directory.
		/// </summary>
		public bool UseTempPath { get; set; }

		/// <summary>
		/// The JSON file name to store scan results.
		/// </summary>
		public string JsonOutputFileName { get; set; } = "DirectoryScannerOutput.json";

		/// <summary>
		/// Returns the full path to the output file, resolved at runtime - temp path or the base assembly directory
		/// </summary>
		public string GetResolvedOutputPath()
		{
			return Path.Combine(UseTempPath ? Path.GetTempPath() : AppContext.BaseDirectory, JsonOutputFileName);
		}
	}
}
