namespace Inss.Common;

public readonly record struct Result<TValue>
{
    private readonly TValue? _value = default;
    private readonly Error? _error = null;
    
    private Result(TValue value)
    {
        _value = value;
    }

    private Result(Error error)
    {
        _error = error;
    }

    public bool IsError => _error is not null;
    
    public static implicit operator Result<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }
    
    public static implicit operator TValue?(Result<TValue> result)
    {
        return result._value;
    }
    
    public static implicit operator Result<TValue>(Error error)
    {
        return new Result<TValue>(error);
    }

    public static implicit operator Error?(Result<TValue> result)
    {
        return result._error;
    }
    
    public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onError)
    {
        return IsError ? onError(_error!.Value) : onValue(_value!);
    }
    
    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, Task<TResult>> onValue, 
        Func<Error, Task<TResult>> onError)
    {
        return IsError 
            ? await onError(_error!.Value).ConfigureAwait(false) 
            : await onValue(_value!).ConfigureAwait(false);
    }
}