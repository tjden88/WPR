namespace WPR.Domain.Interfaces;

/// <summary>
/// Интерфейс фильтра файлов
/// </summary>
public interface IFileFilter
{
    /// <summary>
    /// Список паттернов поиска совпадений файлов
    /// Пример паттерна: *.jpg
    /// </summary>
    IEnumerable<string> FileMathPattrerns { get; }


    /// <summary> Описание фильтра </summary>
    string Description { get; }
        
}