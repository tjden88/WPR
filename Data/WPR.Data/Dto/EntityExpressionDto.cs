namespace WPR.Data.Dto;

/// <summary>
/// Dto для передачи сущностей с выражениями
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityExpressionDto<T>
{
    public T Item { get; set; }

    public IEnumerable<QueryExpressionDto> Expressions { get; set; }
}