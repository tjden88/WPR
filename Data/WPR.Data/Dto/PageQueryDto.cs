namespace WPR.Data.Dto;

/// <summary>
/// Dto для передачи данных выборки страницы, выражений фильтра и значений сортировки
/// </summary>
public class PageQueryDto
{
    /// <summary> Номер страницы (с нуля) </summary>
    public int PageIndex { get; set; }

    /// <summary> Размер страницы </summary>
    public int PageSize { get; set; }

    /// <summary> Выражение фильтрации выборки </summary>
    public QueryExpressionDto? Filter { get; set; }

    /// <summary> Первичная сортировка </summary>
    public OrderQuery? Order { get; set; }

    /// <summary> Список сортировок, идущих после первичной </summary>
    public List<OrderQuery>? ThenOrder { get; set; }


    /// <summary>
    /// Запрос сортировки данных
    /// </summary>
    /// <param name="OrderBy">Выражение сортировки</param>
    /// <param name="Ascending">По возрастанию или по убыванию</param>
    public record OrderQuery(QueryExpressionDto OrderBy, bool Ascending);
}