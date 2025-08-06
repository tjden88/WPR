using WPR.AppConfig.Implementations;

namespace WPR.AppConfig;

/// <summary>
/// Построитель поставщиков настроек приложения
/// </summary>
public class ConfigBuilder
{
    private ConfigRoot ConfigRoot { get; } = new();

    /// <summary>
    /// Получить агрегатор настроек приложения
    /// </summary>
    public IConfigRoot Build() => ConfigRoot;


    /// <summary>
    /// Добавить поставщика настроек
    /// </summary>
    /// <typeparam name="T">Объект с настройками</typeparam>
    /// <param name="config">Поставщик настроек</param>
    public ConfigBuilder Add<T>(IConfig<T> config)
    {
        ConfigRoot.AddConfig(config);
        return this;
    }
}