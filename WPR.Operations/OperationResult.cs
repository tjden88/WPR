namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, не возвращающих значение, с возможностью указания ошибки.
/// </summary>
public readonly struct OperationResult : IOperationResult<string>
{
    private OperationResult(bool success, string error)
    {
        IsSuccess = success;
        Error = error;
    }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась неудачей.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    public string Error { get; init; }

    #region Factory

    /// <summary>
    /// Создает успешный результат операции.
    /// </summary>
    public static OperationResult Success() => new(true, string.Empty);


    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке.
    /// </summary>
    public static OperationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return new OperationResult(false, error);
    }

    #endregion

    #region Operators

    public static implicit operator OperationResult(bool success) => success ? Success() : Failure("Операция завершилась с ошибкой");

    public static implicit operator bool(OperationResult result) => result.IsSuccess;

    #endregion


    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure (\"{Error}\")";
}