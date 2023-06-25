using System.Linq.Expressions;

namespace WPR.Data.Paging.Interfaces;

/// <summary>
/// Сведения о сортировке при формировании страницы
/// </summary>
public interface IPageOrderInfo<T>
{
    /// <summary>
    /// Выражение сортировки объекта
    /// </summary>
    Expression<Func<T, object>> OrderExpression { get; }


    /// <summary>
    /// Порядок сортировки по возрастанию или убыванию
    /// </summary>
    bool Ascending { get; }
}