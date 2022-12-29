namespace WPR.Data.Paging.Interfaces;

public interface IPage
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    int PageIndex { get; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Всего страниц
    /// </summary>
    public int TotalPagesCount { get; }

    /// <summary>
    /// Есть предыдущая страница
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;

    /// <summary>
    /// Есть следующая страница
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPagesCount - 1;

    /// <summary>
    /// Всего больше одной страницы
    /// </summary>
    public bool HasManyPages => TotalPagesCount > 1;
}