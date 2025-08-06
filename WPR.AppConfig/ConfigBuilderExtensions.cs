using System.Text.Json;
using WPR.AppConfig.Implementations;

namespace WPR.AppConfig;

public static class ConfigBuilderExtensions
{

    /// <summary>
    /// Добавить настройки с сохранением и загрузкой из json файла.
    /// Можно добавить несколько классов с настройками для одного json файла
    /// </summary>
    /// <param name="Builder">ConfigBuilder</param>
    /// <param name="options">Опции настроек</param>
    /// <returns></returns>
    public static ConfigBuilder AddJsonFile(this ConfigBuilder Builder, Action<JsonFileConfigOptions> options)
    {
        options.Invoke(new JsonFileConfigOptions(Builder));
        return Builder;
    }

    /// <summary>
    /// Добавить настройки с сохранением и загрузкой из json файла
    /// </summary>
    /// <typeparam name="T">Объект настроек</typeparam>
    /// <param name="Builder">ConfigBuilder</param>
    /// <param name="FilePath">Имя файла настроек. По умолчанию: Config.json</param>
    /// <param name="options">Опции сериализатора</param>
    /// <param name="SectionName">Имя секции в json файле. Если не задано - будет использоваться имя типа настроек</param>
    public static ConfigBuilder AddJsonFile<T>(this ConfigBuilder Builder, string FilePath = "Config.json", JsonSerializerOptions? options = null, string? SectionName = null) where T : class, new() =>
        Builder.Add(new JsonFileConfig<T>(FilePath, options));


    /// <summary>
    /// Добавить настройки, которые хранятся в памяти приложения.
    /// При вызове Load() создаётся новый объект.
    /// Метод Save() ничего не делает
    /// </summary>
    public static ConfigBuilder AddMemoryConfig<T>(this ConfigBuilder Builder) where T : new() => Builder.Add(new MemoryAppConfig<T>());

}