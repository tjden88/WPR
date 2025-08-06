namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, возвращающих значение типа T с возможностью указания ошибки.
/// </summary>
public readonly struct OperationResult<T> : IOperationResult<string>
{
    private OperationResult(T value, string error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Значение операции, если она успешна.
    /// </summary>
    public T Value { get; init; }

    /// <summary>
    /// Сообщение об ошибке, если операция завершилась неудачей.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    public bool IsSuccess => string.IsNullOrWhiteSpace(Error);

    /// <summary>
    /// Указывает, что операция завершилась неудачей.
    /// </summary>
    public bool IsFailure => !IsSuccess;


    #region Factory

    /// <summary>
    /// Создает успешную операцию с заданным значением.
    /// </summary>
    /// <param name="value">Обязательное значение</param>
    public static OperationResult<T> Success(T value)
    {
        if (value is null && Nullable.GetUnderlyingType(typeof(T)) == null && !typeof(T).IsClass)
            throw new ArgumentNullException(nameof(value));
        

        return new OperationResult<T>(value, string.Empty);
    }

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке.
    /// </summary>
    /// <param name="error">Текст ошибки</param>
    /// <returns></returns>
    public static OperationResult<T> Failure(string error)
    {
        if(string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return new OperationResult<T>(default!, error);
    }

    #endregion

    #region Operators

    // Неявное преобразование из T
    public static implicit operator OperationResult<T>(T value) => Success(value);

    // Неявное преобразование в T
    public static implicit operator T(OperationResult<T> op) => op.Value;

    // Неявное преобразование к bool — для `if (operation)`
    public static implicit operator bool(OperationResult<T> op) => op.IsSuccess;

    #endregion


    // Деструктуризация
    public void Deconstruct(out T? value, out string? error)
    {
        value = Value;
        error = Error;
    }

    public override string ToString()
    {
        return IsSuccess
            ? $"Success ({Value})"
            : $"Failure (\"{Error}\")";
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или выбрасывает исключение, если неудачна.
    /// </summary>
    public T GetValueOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Операция завершилась неудачей: {Error}");
        return Value!;
    }

    /// <summary>
    /// Получает значение операции, если она успешна, или возвращает значение по умолчанию, если неудачна.
    /// </summary>
    public T GetValueOrDefault(T fallback = default!) => IsSuccess ? Value : fallback;
}
