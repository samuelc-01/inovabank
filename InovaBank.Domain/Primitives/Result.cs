namespace InovaBank.Domain.Primitives;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public int StatusCode { get; }

    protected Result(bool isSuccess, string? error, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
    }

    public static Result Success() => new(true, null, 200);
    public static Result Failure(string error, int statusCode = 400) => new(false, error, statusCode);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error, int statusCode) : base(isSuccess, error, statusCode)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null, 200);
    public static Result<T> Created(T value) => new(true, value, null, 201);
    public static new Result<T> Failure(string error, int statusCode = 400) => new(false, default, error, statusCode);
}
