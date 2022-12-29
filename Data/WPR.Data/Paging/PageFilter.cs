using System.Linq.Expressions;
using WPR.Data.Paging.Interfaces;

namespace WPR.Data.Paging;

/// <summary>
/// Инфо для запроса страницы из репозитория
/// </summary>
public class PageFilter<T> : IPageFilter<T>
{
    /// <summary>
    /// Номер запрашиваемой страницы
    /// </summary>
    public int PageIndex { get; set; }


    /// <summary>
    /// Размер страницы
    /// </summary>
    public int PageSize { get; set; }


    /// <summary>
    /// Фильтр выборки элементов
    /// </summary>
    public Expression<Func<T, bool>>? Filter { get; set; }


    /// <summary>
    /// Первичная сортировка элементов
    /// </summary>
    public IPageOrderInfo<T>? OrderBy { get; set; }


    /// <summary>
    /// Список дополнительной сортировки элементов
    /// </summary>
    public ICollection<IPageOrderInfo<T>>? ThenOrderBy { get; set; }

}