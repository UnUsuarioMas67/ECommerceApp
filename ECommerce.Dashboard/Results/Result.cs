namespace ECommerce.Dashboard.Results;

public record Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error ?? Error.None();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    private readonly T? _value;
    public T Value => IsSuccess
            ? _value ?? throw new InvalidOperationException($"{nameof(Value)} cannot be null")
            : throw new InvalidOperationException("Tried to get a Value from a failed Result");

    private Result(T value) : base(true, null) => _value = value;
    private Result(Error error) : base(false, error) { }

    public static Result<T> Success(T value) => new(value ?? throw new ArgumentNullException(nameof(value)));
    public new static Result<T> Failure(Error error) => new(error ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(Error error) => new(error);
}