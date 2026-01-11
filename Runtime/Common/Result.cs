using System;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// A Result type that represents either a success with a value or a failure with an error.
    /// This pattern eliminates null returns and provides explicit error handling.
    /// Follows the Railway Oriented Programming pattern for error handling.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The success value. Only valid when IsSuccess is true.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// The error message. Only valid when IsFailure is true.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Detailed error information including the original exception if available.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// HTTP status code if the error was from an HTTP response.
        /// </summary>
        public int? HttpStatusCode { get; }

        private Result(T value, bool isSuccess, string error, Exception exception = null, int? httpStatusCode = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Exception = exception;
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        /// <param name="value">The success value.</param>
        /// <returns>A successful Result containing the value.</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, null);
        }

        /// <summary>
        /// Creates a failure result with the given error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="exception">Optional exception that caused the failure.</param>
        /// <param name="httpStatusCode">Optional HTTP status code.</param>
        /// <returns>A failure Result containing the error information.</returns>
        public static Result<T> Failure(string error, Exception exception = null, int? httpStatusCode = null)
        {
            return new Result<T>(default, false, error, exception, httpStatusCode);
        }

        /// <summary>
        /// Executes the appropriate action based on the result state.
        /// </summary>
        /// <param name="onSuccess">Action to execute if successful.</param>
        /// <param name="onFailure">Action to execute if failed.</param>
        public void Match(Action<T> onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess?.Invoke(Value);
            else
                onFailure?.Invoke(Error);
        }

        /// <summary>
        /// Maps the success value to a new value using the provided function.
        /// </summary>
        /// <typeparam name="TResult">The type of the new value.</typeparam>
        /// <param name="mapper">Function to transform the value.</param>
        /// <returns>A new Result with the mapped value or the original error.</returns>
        public Result<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (IsFailure)
                return Result<TResult>.Failure(Error, Exception, HttpStatusCode);

            try
            {
                return Result<TResult>.Success(mapper(Value));
            }
            catch (Exception ex)
            {
                return Result<TResult>.Failure(ex.Message, ex);
            }
        }

        /// <summary>
        /// Returns the value if successful, otherwise returns the default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return on failure.</param>
        /// <returns>The success value or the default value.</returns>
        public T GetValueOrDefault(T defaultValue = default)
        {
            return IsSuccess ? Value : defaultValue;
        }

        /// <summary>
        /// Implicitly converts a value to a successful Result.
        /// </summary>
        public static implicit operator Result<T>(T value)
        {
            return Success(value);
        }
    }

    /// <summary>
    /// Represents a Result without a value, used for operations that only need to indicate success or failure.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the operation failed.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The error message. Only valid when IsFailure is true.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Detailed error information including the original exception if available.
        /// </summary>
        public Exception Exception { get; }

        private Result(bool isSuccess, string error, Exception exception = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Exception = exception;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>A successful Result.</returns>
        public static Result Success()
        {
            return new Result(true, null);
        }

        /// <summary>
        /// Creates a failure result with the given error message.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <param name="exception">Optional exception that caused the failure.</param>
        /// <returns>A failure Result containing the error information.</returns>
        public static Result Failure(string error, Exception exception = null)
        {
            return new Result(false, error, exception);
        }

        /// <summary>
        /// Executes the appropriate action based on the result state.
        /// </summary>
        /// <param name="onSuccess">Action to execute if successful.</param>
        /// <param name="onFailure">Action to execute if failed.</param>
        public void Match(Action onSuccess, Action<string> onFailure)
        {
            if (IsSuccess)
                onSuccess?.Invoke();
            else
                onFailure?.Invoke(Error);
        }
    }
}
