namespace WPR.Abstractions.Interfaces;

/// <summary>
/// Отложенная загрузка зависимостей.
/// Для предотвращения циклических зависимостей, для получения нескольких экземпляров одного типа
/// </summary>
public interface IResolver<out T> where T : notnull
{
    /// <summary>
    /// Получить значение
    /// </summary>
    /// <returns></returns>
    T GetValue();
}