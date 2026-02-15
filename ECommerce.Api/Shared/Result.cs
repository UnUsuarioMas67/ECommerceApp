namespace ECommerce.Api.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Dictionary<string, string[]> Errors { get; }

    protected Result(bool success, T? value, Dictionary<string, string[]>? errors)
    {
        IsSuccess = success;
        Value = value;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    public static Result<T> Success(T value)
        => new Result<T>(true, value, null);

    public static Result<T?> Fail(string error)
    {
        var errors = new Dictionary<string, string[]>
        {
            { "", [error] }
        };

        return new Result<T?>(false, default, errors);
    }

    public static Result<T?> Fail(Dictionary<string, string[]> error)
        => new Result<T?>(false, default, error);
}