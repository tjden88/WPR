namespace WPR.Operations;

/// <summary>
/// Фабричные методы для создания результатов операций.
/// </summary>
public static class Result
{
    /// <summary>
    /// Создает успешный результат операции без значения.
    /// </summary>
    public static OperationResult Success() => OperationResult.Success();

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке.
    /// </summary>
    public static OperationResult Failure(string error) => OperationResult.Failure(error);

    /// <summary>
    /// Создает успешный результат операции с заданным значением типа <typeparamref name="T"/>.
    /// </summary>
    public static OperationResult<T> Success<T>(T value) => OperationResult<T>.Success(value);

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке для типа <typeparamref name="T"/>.
    /// </summary>
    public static OperationResult<T> Failure<T>(string error) => OperationResult<T>.Failure(error);

    /// <summary>
    /// Создает успешный результат операции с заданным значением типа <typeparamref name="T"/>
    /// и ошибкой типа <typeparamref name="TError"/>.
    /// </summary>
    public static OperationResult<T, TError> Success<T, TError>(T value) => OperationResult<T, TError>.Success(value);

    /// <summary>
    /// Создает неудачный результат операции с заданной ошибкой типа <typeparamref name="TError"/>.
    /// </summary>
    public static OperationResult<T, TError> Failure<T, TError>(TError error) => OperationResult<T, TError>.Failure(error);

    /// <summary>
    /// Создает результат операции на основе булевого значения и сообщения об ошибке.
    /// </summary>
    /// <param name="value"><see langword="true"/> — успех, <see langword="false"/> — неудача.</param>
    /// <param name="error">
    /// Сообщение об ошибке. Проверяется и требуется только при <paramref name="value"/> = <see langword="false"/>;
    /// при успехе значение игнорируется.
    /// </param>
    public static OperationResult FromResult(bool value, string error)
        => value ? Success() : Failure(error);

    /// <summary>
    /// Создает результат операции на основе булевого значения, значения типа <typeparamref name="T"/>
    /// и сообщения об ошибке.
    /// </summary>
    /// <param name="success"><see langword="true"/> — успех, <see langword="false"/> — неудача.</param>
    /// <param name="value">Значение при успехе. Не может быть <see langword="null"/> при <paramref name="success"/> = <see langword="true"/>.</param>
    /// <param name="error">
    /// Сообщение об ошибке. Проверяется и требуется только при <paramref name="success"/> = <see langword="false"/>;
    /// при успехе значение игнорируется.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="success"/> = <see langword="true"/>, но <paramref name="value"/> равно <see langword="null"/>.
    /// </exception>
    public static OperationResult<T> FromResult<T>(bool success, T? value, string error)
    {
        if (success)
        {
            if (value is not { } val)
                throw new ArgumentException("Значение не может быть null при успешном результате", nameof(value));

            return Success(val);
        }

        return Failure<T>(error);
    }

    /// <summary>
    /// Создает результат операции на основе значения типа <typeparamref name="T"/> и сообщения об ошибке.
    /// Успех — если значение не <see langword="null"/>, иначе — неудача с указанной ошибкой.
    /// </summary>
    /// <param name="value">Значение операции. <see langword="null"/> означает неудачу.</param>
    /// <param name="error">
    /// Сообщение об ошибке. Проверяется и требуется только когда <paramref name="value"/> равно <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// Для значимых типов значение по определению не равно <see langword="null"/>, поэтому метод всегда
    /// вернёт успех. Для явной проверки «пустого» значения используйте
    /// <see cref="FromResult{T}(bool, T, string)"/>.
    /// </remarks>
    public static OperationResult<T> FromResult<T>(T? value, string error)
        => value is not null ? Success(value) : Failure<T>(error);
}
