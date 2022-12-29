using WPR.Data.Paging.Interfaces;

namespace WPR.Data.Paging;

/// <summary>
/// Реализация интерфейса постраничной выборки элементов репозиториев
/// </summary>
public record Page<T>(IEnumerable<T> Items, int TotalCount, int PageIndex, int PageSize) : IPage<T>
{
    public int TotalPagesCount => PageSize < 1
        ? 0
        : (int)Math.Ceiling((double)TotalCount / PageSize + 1) - 1;


    /// <summary> Создать копию страницы с другими элементами </summary>
    public IPage<TOut> Map<TOut>(IEnumerable<TOut> Items) => new Page<TOut>(Items, TotalCount, PageIndex, PageSize);
}