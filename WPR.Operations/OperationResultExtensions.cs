namespace WPR.Operations;

/// <summary>
/// Расширения для <see cref="OperationResult"/>.
/// </summary>
public static class OperationResultExtensions
{
    /// <summary>
    /// Преобразует результат операции без значения в результат операции с указанным значением.
    /// </summary>
    /// <typeparam name="T">Тип значения результирующей операции.</typeparam>
    /// <param name="result">Исходный результат.</param>
    /// <param name="value">Значение для успешного результата.</param>
    /// <returns>
    /// Успешный <see cref="OperationResult{T}"/> со значением <paramref name="value"/>,
    /// если <paramref name="result"/> успешен; иначе — неудачный результат с исходной ошибкой.
    /// </returns>
    public static OperationResult<T> As<T>(this OperationResult result, T value = default!)
    {
        return result.IsSuccess
            ? OperationResult<T>.Success(value)
            : OperationResult<T>.Failure(
                string.IsNullOrWhiteSpace(result.Error)
                    ? "Операция завершилась с ошибкой"
                    : result.Error);
    }
}
