using System.Linq.Expressions;
using WPR.Data.Paging.Interfaces;

namespace WPR.Data.Paging;

/// <summary>
/// Сведения о сортировке при формировании страницы
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
/// <param name="OrderExpression">Выражение сортировки объекта</param>
/// <param name="Ascending">орядок сортировки по возрастанию (по умолчанию - true)</param>
public record PageOrderInfo<T>(Expression<Func<T, object>> OrderExpression, bool Ascending = true) : IPageOrderInfo<T>;