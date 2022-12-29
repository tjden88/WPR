using System.Linq.Expressions;
using WPR.Data.Paging.Interfaces;

namespace WPR.Data.Paging;

/// <summary>
/// Сведения о сортировке при формировании страницы
/// </summary>
public class PageOrderInfo<T> : IPageOrderInfo<T>
{
    public PageOrderInfo()
    {
    }

    public PageOrderInfo(Expression<Func<T, object>> OrderExpression, bool Ascending = true)
    {
        this.OrderExpression = OrderExpression;
        this.Ascending = Ascending;
    }

    /// <summary>
    /// Выражение сортировки объекта
    /// </summary>
    public Expression<Func<T, object>> OrderExpression { get; set; }


    /// <summary>
    /// Порядок сортировки по возрастанию (по умолчанию - true)
    /// </summary>
    public bool Ascending { get; set; } = true;
}