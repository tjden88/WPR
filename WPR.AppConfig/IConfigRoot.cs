
namespace WPR.AppConfig;

/// <summary>
/// Содержит настройки всех поставщиков конфигурации приложения
/// </summary>
public interface IConfigRoot
{
    /// <summary>
    /// Поставщики настроек
    /// </summary>
    IEnumerable<IConfigProvider> Providers { get; }

    /// <summary>
    /// Получить настройки добавленного поставщика настроек
    /// </summary>
    T Get<T>();

    /// <summary>
    /// Сохраняет текущую конфигурацию выбранного поставщика
    /// </summary>
    void Save<T>();

    /// <summary>
    /// Перезагружает конфигурацию выбранного поставщика
    /// </summary>
    void Reload<T>();
}