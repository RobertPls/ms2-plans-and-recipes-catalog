namespace Catalog.Application.Utils
{
    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public Result() { }

        public Result(bool success, string? message = null)
        {
            Success = success;
            Message = message;
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public Result() { }

        public Result(bool success, string? message = null, T? value = default) : base(success, message)
        {
            Value = value;
        }

        public Result(T value) : base(true)
        {
            Value = value;
        }
    }
}
