namespace WPR.Data.Dto;

/// <summary>
/// Dto для передачи сущности с LINQ выражениями
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
/// <param name="Item">Экземпляр сущности</param>
public record EntityExpressionDto<T>(T Item)
{
    /// <summary> Выражения LINQ для экземпляра сущности </summary>
    public IEnumerable<QueryExpressionDto> Expressions { get; set; } = Enumerable.Empty<QueryExpressionDto>();
}