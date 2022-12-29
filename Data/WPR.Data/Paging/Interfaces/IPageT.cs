namespace WPR.Data.Paging.Interfaces;

public interface IPage<out T> : IPage
{
    /// <summary> Сущности страницы </summary>
    IEnumerable<T> Items { get; }

    /// <summary>
    /// Общее количество сущностей
    /// </summary>
    int TotalCount { get; }


    /// <summary>
    /// Создать копию страницы с другими элементами
    /// </summary>
    /// <param name="Items">Новая коллекция элементов</param>
    /// <returns></returns>
    IPage<TOut> Map<TOut>(IEnumerable<TOut> Items);
}