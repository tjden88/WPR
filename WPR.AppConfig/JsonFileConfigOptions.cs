using System.Text.Json;
using WPR.AppConfig.Implementations;

namespace WPR.AppConfig;

/// <summary>
/// Конфигурация для json-файла настроек
/// </summary>
/// <param name="Builder"></param>
public class JsonFileConfigOptions(ConfigBuilder Builder)
{

    /// <summary>
    /// Имя файла настроек. По умолчанию: Config.json
    /// По умолчанию располагается вместе с исполняемым файлом
    /// </summary>
    public string FilePath { get; set; } = "Config.json";

    /// <summary>
    /// Опции сериализатора
    /// </summary>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// Добавить тип данных настроек
    /// </summary>
    /// <param name="SectionName">Имя секции в json файле. Если не задано - будет использоваться имя типа настроек</param>
    public void AddConfig<T>(string? SectionName = null) where T : class, new() => Builder.Add(new JsonFileConfig<T>(FilePath, JsonSerializerOptions, SectionName));
}