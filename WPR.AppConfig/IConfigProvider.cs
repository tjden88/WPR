namespace WPR.AppConfig;

/// <summary>
/// Поставщик конфигурации приложения
/// </summary>
public interface IConfigProvider
{
    Type ConfigType { get; }

    /// <summary>
    /// Объект с настройками
    /// </summary>
    object Config { get; }

    /// <summary>
    /// Сохраняет текущую конфигурацию.
    /// </summary>
    void Save();


    /// <summary>
    /// Загружает конфигурацию.
    /// </summary>
    void Reload();

}