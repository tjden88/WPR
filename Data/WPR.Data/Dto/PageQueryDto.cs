namespace WPR.Data.Dto;

/// <summary>
/// Dto для передачи данных выборки страницы, выражений фильтра и значений сортировки
/// </summary>
public class PageQueryDto
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public QueryExpressionDto Filter { get; set; }

    public OrderQuery Order { get; set; }

    public List<OrderQuery> ThenOrder { get; set; }


    public record OrderQuery(QueryExpressionDto OrderBy, bool Ascending);
}