namespace WPR.AppConfig;

/// <summary>
/// Контракт для конфигурации приложения.
/// </summary>
/// <typeparam name="T">Объект данных для сохранения и загрузки конфигурации</typeparam>
public interface IConfig<out T> : IConfigProvider
{

    Type IConfigProvider.ConfigType => typeof(T);

    object IConfigProvider.Config => Config!;

    /// <summary>
    /// Объект с настройками
    /// </summary>
    public new T Config { get; }
}