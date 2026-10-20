namespace AzertaNodes.Core.Entities
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure { get; }
        public T? Value { get; }
        public Error? Error { get; }
        protected Result(bool success, T? value, Error? error)
        {
            IsSuccess = success;
            IsFailure = !success;
            Value = value;
            Error = error;
        }
        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(Error error) => new(false, default, error);
        public T GetValueOrThrow()
        {
            if (IsFailure) throw new InvalidOperationException($"Cannot get value from a failed result. Error: {Error?.UserErrorMessage}");
            return Value!;
        }
    }

    public class NullResultException(string message) : Exception(message) { }
}