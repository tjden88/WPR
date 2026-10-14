using System.Text.Json.Serialization;

namespace WPR.Operations;

/// <summary>
/// Универсальная обертка для операций, не возвращающих значение, с возможностью указания ошибки.
/// </summary>
/// <remarks>
/// Единственным источником истины об успехе является <see cref="Error"/>:
/// успешная операция всегда содержит пустую строку (см. <see cref="Success"/>),
/// любое другое значение трактуется как неудача. Значение <c>default(OperationResult)</c>
/// имеет <see cref="Error"/> = <see langword="null"/> и поэтому считается неудачей (fail-safe).
/// Так как <see cref="IsSuccess"/> вычисляется из <see cref="Error"/>, состояние невозможно
/// рассогласовать объектным инициализатором, выражением <c>with</c> или десериализацией JSON.
/// </remarks>
public readonly struct OperationResult : IOperationResult<string>
{
    private OperationResult(string error) => Error = error;

    /// <summary>
    /// Сообщение об ошибке, если операция завершилась неудачей, либо пустая строка при успехе.
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно (ошибка не задана).
    /// </summary>
    public bool IsSuccess => Error is { Length: 0 };

    /// <summary>
    /// Указывает, что операция завершилась неудачей.
    /// </summary>
    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    #region Factory

    /// <summary>
    /// Создает успешный результат операции.
    /// </summary>
    public static OperationResult Success() => new(string.Empty);

    /// <summary>
    /// Создает неудачный результат операции с заданным сообщением об ошибке.
    /// </summary>
    /// <param name="error">Текст ошибки. Не может быть <see langword="null"/>, пустым или состоять только из пробелов.</param>
    /// <exception cref="ArgumentException">Сообщение об ошибке пустое или состоит только из пробелов.</exception>
    public static OperationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Сообщение об ошибке не может быть пустым", nameof(error));

        return new OperationResult(error);
    }

    #endregion

    #region Operators

    /// <summary>
    /// Неявно превращает булево значение в результат операции:
    /// <see langword="true"/> — успех, <see langword="false"/> — неудача со стандартным сообщением.
    /// </summary>
    public static implicit operator OperationResult(bool success) => success ? Success() : Failure("Операция завершилась с ошибкой");

    /// <summary>
    /// Неявно превращает результат операции в признак успеха (для <c>if (result)</c>).
    /// </summary>
    public static implicit operator bool(OperationResult result) => result.IsSuccess;

    #endregion

    /// <inheritdoc />
    public override string ToString() =>
        IsSuccess ? "Success" : $"Failure (\"{Error}\")";
}
