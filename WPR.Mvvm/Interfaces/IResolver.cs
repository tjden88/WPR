namespace WPR.Mvvm.Interfaces;

/// <summary>
/// Доступ к сервисам приложения, для отложенной или предотвращения циклической зависимости.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResolver<out T> where T : notnull
{
    /// <summary>
    /// Получить экземпляр из сервисов приложения
    /// </summary>
    T Get();
    
    /// <summary>
    /// Получить именованный экземпляр из сервисов приложения
    /// </summary>
    /// <param name="name">Именованный параметр сервиса</param>
    T Get(string name);

    /// <summary>
    /// Получить сконфигурированный экземпляр из коллекции сервисов
    /// </summary>
    /// <param name="configure">действие конфигурации</param>
    T Get(Action<T> configure);

    //IDisposable GetScope(out T value); - на будущее, если понадобится поддержка
}