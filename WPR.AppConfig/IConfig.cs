namespace WPR.AppConfig;

/// <summary>
/// Контракт для конфигурации приложения.
/// </summary>
/// <typeparam name="T">Объект данных для сохранения и загрузки конфигурации</typeparam>
public interface IConfig<out T> : IConfigProvider
{

    /// <summary>
    /// Объект с настройками
    /// </summary>
    public new T Config { get; }
}