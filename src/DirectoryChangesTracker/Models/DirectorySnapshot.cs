﻿using System.ComponentModel.DataAnnotations;

namespace DirectoryChangesTracker.Models
{
	/// <summary>
	/// Represents a snapshot of a single directory
	/// </summary>
	public class DirectorySnapshot
	{
        /// <summary>
        /// Full local path to the scanned directory.
        /// </summary>
        [Display(Name = "Local directory path")]
        [Required(ErrorMessage = "Insert the local directory path")]
        public required string LocalPath { get; set; }

		/// <summary>
		/// Date and time when this directory was scanned for the first time.
		/// </summary>
		public DateTime FirstListing { get; set; } = DateTime.Now;

		/// <summary>
		/// Date and time when this directory was scanned most recently.
		/// </summary>
		public DateTime LastListing { get; set; }

		/// <summary>
		/// Total number of times this directory has been scanned.
		/// </summary>
		public int ListingsCount { get; set; }

		/// <summary>
		/// Collection of file snapshots captured during the scan.
		/// </summary>
		public HashSet<FileSnapshot> Files { get; set; } = new();

		/// <summary>
		/// Collection of subdirectory names captured during the scan.
		/// </summary>
		public HashSet<string> SubDirectories { get; set; } = new();
	}
}
