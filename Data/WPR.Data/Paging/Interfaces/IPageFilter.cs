using System.Linq.Expressions;

namespace WPR.Data.Paging.Interfaces;

/// <summary>
/// Инфо для запроса страницы из репозитория
/// </summary>
public interface IPageFilter<T>
{
    /// <summary>
    /// Номер запрашиваемой страницы
    /// </summary>
    int PageIndex { get; }


    /// <summary>
    /// Размер страницы
    /// </summary>
    int PageSize { get; }


    /// <summary>
    /// Фильтр выборки элементов
    /// </summary>
    Expression<Func<T, bool>>? Filter { get; }


    /// <summary>
    /// Первичная сортировка элементов
    /// </summary>
    IPageOrderInfo<T>? OrderBy { get; }


    /// <summary>
    /// Список дополнительной сортировки элементов
    /// </summary>
    ICollection<IPageOrderInfo<T>>? ThenOrderBy { get; }
}