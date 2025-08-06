using System.Collections;

namespace WPR.Repository.Abstractions;

/// <summary>
/// Реализация интерфейса постраничной выборки элементов репозиториев
/// </summary>
public record Page<T> : IPage<T>
{
    private readonly IList<T> _Items;

    public Page(IList<T> Items, int TotalItemsCount, int PageIndex, int PageSize)
    {
        _Items = Items.ToList();
        this.TotalItemsCount = TotalItemsCount;
        this.PageIndex = PageIndex;
        this.PageSize = PageSize;
    }


    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalItemsCount { get; }


    public int TotalPagesCount => PageSize < 1
        ? 0
        : (int)Math.Ceiling((double)TotalItemsCount / PageSize + 1) - 1;


    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPagesCount;
    public bool HasManyPages => TotalPagesCount > 1;


    #region IReadOnlyList Implementation

    public IEnumerator<T> GetEnumerator() => _Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => _Items.Count;
    public T this[int index] => _Items[index];

    #endregion

}