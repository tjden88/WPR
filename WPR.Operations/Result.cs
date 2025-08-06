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
    /// Создает успешный результат операции с заданным значением типа T.
    /// </summary>
    public static OperationResult<T> Success<T>(T value) => OperationResult<T>.Success(value);

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке для типа T.
    /// </summary>
    public static OperationResult<T> Failure<T>(string error) => OperationResult<T>.Failure(error);

    /// <summary>
    /// Создает успешный результат операции с заданным значением типа T и ошибкой типа TError.
    /// </summary>
    public static OperationResult<T, TError> Success<T, TError>(T value) => OperationResult<T, TError>.Success(value);

    /// <summary>
    /// Создает неудачный результат операции с заданной ошибкой типа TError.
    /// </summary>
    public static OperationResult<T, TError> Failure<T, TError>(TError error) => OperationResult<T, TError>.Failure(error);

    /// <summary>
    /// Создает результат операции на основе булевого значения и сообщения об ошибке.
    /// </summary>
    public static OperationResult FromResult(bool value, string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return value ? Success() : Failure(error);
    }


    /// <summary>
    /// Создает результат операции на основе булевого значения, значения типа T и сообщения об ошибке.
    /// </summary>
    public static OperationResult<T> FromResult<T>(bool success, T? value, string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        if (success)
        {
            if (value is not { } val)
                throw new ArgumentException("Значение не может быть null при успешном результате", nameof(value));
            return Success(val);
        }

        return Failure<T>(error);
    }


    /// <summary>
    /// Создает результат операции на основе значения типа T и сообщения об ошибке.
    /// Успех - если значение не null, иначе - неудача с указанной ошибкой.
    /// </summary>
    public static OperationResult<T> FromResult<T>(T? value, string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return value != null ? Success(value) : Failure<T>(error);
    }
}