namespace Finances.Domain.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);
}

public sealed class Result<T> : Result
{
    private readonly T? value;

    private Result(T value)
        : base(true, null)
    {
        this.value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
    }

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("No se puede acceder al valor de un resultado fallido.");

    public static Result<T> Success(T value) => new(value);

    public new static Result<T> Failure(Error error) => new(error);
}
