namespace WPR.Operations;

public interface IOperationResult<TError>
{
    /// <summary>
    /// Сообщение об ошибке, если операция завершилась неудачей.
    /// </summary>
    TError Error { get; init; }

    /// <summary>
    /// Указывает, что операция завершилась успешно.
    /// </summary>
    bool IsSuccess { get; }
}