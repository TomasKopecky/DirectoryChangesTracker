namespace DirectoryChangesTracker.Models
{
	/// <summary>
	/// Represents the result of an operation that may either succeed or fail, and includes a return value.
	/// </summary>
	/// <typeparam name="T">The type of the value returned on success.</typeparam>
	public class Result<T> : Result
	{
		/// <summary>
		/// The value returned by the operation, or <c>default</c> if it failed.
		/// </summary>
		public T? Value { get; }

		/// <summary>
		/// Creates a new instance of <see cref="Result{T}"/>.
		/// </summary>
		private Result(T? value, bool isSuccess, string? errorMessage)
			: base(isSuccess, errorMessage)
		{
			Value = value;
		}

		/// <summary>
		/// Creates a successful result containing a value.
		/// </summary>
		public static Result<T> Success(T value) => new(value, true, null);

		/// <summary>
		/// Creates a failed result with an error message.
		/// </summary>
		public static new Result<T> Failure(string errorMessage) => new(default, false, errorMessage);
	}

	/// <summary>
	/// Represents the result of an operation that may either succeed or fail.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// Indicates whether the operation was successful.
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// The error message if the operation failed; otherwise <c>null</c>.
		/// </summary>
		public string? ErrorMessage { get; }

		/// <summary>
		/// Creates a new instance of <see cref="Result"/>.
		/// </summary>
		protected Result(bool isSuccess, string? errorMessage)
		{
			IsSuccess = isSuccess;
			ErrorMessage = errorMessage;
		}

		/// <summary>
		/// Creates a successful result.
		/// </summary>
		public static Result Success() => new(true, null);

		/// <summary>
		/// Creates a failed result with an error message.
		/// </summary>
		public static Result Failure(string errorMessage) => new(false, errorMessage);
	}
}
