namespace ECommerce.Api.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Dictionary<string, string[]> Errors { get; }
    public string? ErrorMessage => Errors.FirstOrDefault().Value.FirstOrDefault();

    public Result(bool success, T? value, Dictionary<string, string[]>? errors)
    {
        IsSuccess = success;
        Value = value;
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}

public static class Result
{
    public static Result<T> Success<T>(T value)
        => new Result<T>(true, value, null);

    public static Result<T> Failure<T>(string error)
    {
        var errors = new Dictionary<string, string[]>
        {
            { "", [error] }
        };

        return new Result<T>(false, default, errors);
    }

    public static Result<T> Failure<T>(Dictionary<string, string[]> error)
        => new Result<T>(false, default, error);
}