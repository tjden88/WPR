namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, возвращающих значение типа T с возможностью указания ошибки типа TError.
/// </summary>
/// <typeparam name="T">Тип значения</typeparam>
/// <typeparam name="TError">Тип ошибки</typeparam>
public readonly struct OperationResult<T, TError> : IOperationResult<TError>
{
    public T? Value { get; init; }
    public TError Error { get; init; }
    public bool IsSuccess => Error == null;

    private OperationResult(T? value, TError error)
    {
        Value = value;
        Error = error;
    }

    public static OperationResult<T, TError> Success(T value) =>
        new(value, default!);

    public static OperationResult<T, TError> Failure(TError error) =>
        new(default, error);

    public void Deconstruct(out T? value, out TError? error)
    {
        value = Value;
        error = Error;
    }

    public override string ToString()
    {
        return IsSuccess
            ? $"Success ({Value})"
            : $"Failure ({Error})";
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или выбрасывает исключение, если неудачна.
    /// </summary>
    public T GetValueOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Operation failed: {Error}");

        return Value!;
    }


    /// <summary>
    /// Получает значение операции, если она успешна, или возвращает значение по умолчанию, если неудачна.
    /// </summary>
    public T? GetValueOrDefault(T? fallback = default) => IsSuccess ? Value : fallback;



    // Неявное преобразование из T
    public static implicit operator OperationResult<T, TError>(T value) => Success(value);

    // Неявное преобразование в T
    public static implicit operator T? (OperationResult<T, TError> op) => op.Value;


    public static implicit operator bool(OperationResult<T, TError> result) => result.IsSuccess;
}
