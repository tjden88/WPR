namespace WPR.Operations;

public static class OperationResultExtensions
{

    /// <summary>
    /// Преобразует результат операции без значения в результат операции с указанным значением.
    /// </summary>
    public static OperationResult<T> As<T>(this OperationResult result, T value = default!)
    {
        return result.IsSuccess
            ? OperationResult<T>.Success(value)
            : OperationResult<T>.Failure(result.Error!);
    }
}