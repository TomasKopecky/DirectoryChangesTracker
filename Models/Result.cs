﻿namespace DirectoryChangesTracker.Models
{
	public class Result<T> : Result
	{
		public T? Value { get; }
		private Result(T? value, bool isSuccess, string? errorMessage) : base(isSuccess, errorMessage)
		{
			Value = value;
		}

		public static Result<T> Success(T value) => new(value, true, null);
		public static new Result<T> Failure(string errorMessage) => new(default, false, errorMessage);
	}

	public class Result
	{
		public bool IsSuccess { get; }
		public string? ErrorMessage { get; }

		protected Result(bool isSuccess, string? errorMessage)
		{
			IsSuccess = isSuccess;
			ErrorMessage = errorMessage;
		}

		public static Result Success() => new(true, null);
		public static Result Failure(string errorMessage) => new(false, errorMessage);
	}
}
