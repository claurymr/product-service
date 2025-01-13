namespace ProductService.Application.Contracts;
public class ResultWithWarning<TValue, TError, TWarning>
{
    public bool IsError { get; }
    public bool IsWarning { get; }
    public bool IsSuccess => !IsError && !IsWarning;

    private readonly TValue? _value;
    private readonly TError? _error;
    private readonly TWarning? _warning;

    public ResultWithWarning(TValue value)
    {
        IsError = false;
        IsWarning = false;
        _value = value;
        _error = default;
        _warning = default;
    }

    public ResultWithWarning(TError error)
    {
        IsError = true;
        IsWarning = false;
        _error = error;
        _value = default;
        _warning = default;
    }

    public ResultWithWarning(TWarning warning)
    {
        IsWarning = true;
        IsError = false;
        _warning = warning;
        _error = default;
        _value = default;
    }

    public static implicit operator ResultWithWarning<TValue, TError, TWarning>(TValue value) => new(value);

    public static implicit operator ResultWithWarning<TValue, TError, TWarning>(TError error) => new(error);

    public static implicit operator ResultWithWarning<TValue, TError, TWarning>(TWarning warning) => new(warning);

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<TError, TResult> failure,
        Func<TWarning, TResult> warn) =>
        !IsError && !IsWarning
        ? success(_value!)
        : (IsError ? failure(_error!) : warn(_warning!));
}