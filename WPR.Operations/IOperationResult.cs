namespace WPR.Operations;

/// <summary>
/// Контракт результата операции с ошибкой типа <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TError">Тип сообщения (или объекта) об ошибке.</typeparam>
public interface IOperationResult<TError>
{
    /// <summary>
    /// Сообщение об ошибке, если операция завершилась неудачей.
    /// Для успешной операции значение должно быть «пустым» для данного типа
    /// (например, <see langword="null"/> или пустая строка).
    /// </summary>
    TError Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    bool IsSuccess { get; }
}
