using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WPR.AppConfig.Implementations;

internal class JsonFileConfig<T>(string FilePath, JsonSerializerOptions? Options = null, string? SectionName = null) : IConfig<T> where T : class, new()
{
    private readonly JsonSerializerOptions _SerializerOptions = Options ?? new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Разрешает кириллицу без экранирова
        WriteIndented = true
    };

    private readonly string _SectionName = SectionName ?? typeof(T).Name;


    private T? _Config;
    public T Config => _Config ??= Load();

    public void Save()
    {
        var root = LoadRawJson();

        // Обновляем нужную секцию
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms, new JsonWriterOptions
        {
            Indented = _SerializerOptions.WriteIndented, // Сохраняем настройки форматирования
            Encoder = _SerializerOptions.Encoder // Используем тот же энкодер для кириллицы
        });

        writer.WriteStartObject();

        foreach (var prop in root.EnumerateObject().Where(prop => !prop.NameEquals(_SectionName)))
        {
            prop.WriteTo(writer);
        }

        writer.WritePropertyName(_SectionName);
        JsonSerializer.Serialize(writer, Config, _SerializerOptions);

        writer.WriteEndObject();
        writer.Flush();

        File.WriteAllText(FilePath, Encoding.UTF8.GetString(ms.ToArray()));
    }
    public void Reload() => _Config = Load();

    private T Load()
    {
        var root = LoadRawJson();
        if (root.TryGetProperty(_SectionName, out var section))
        {
            return JsonSerializer.Deserialize<T>(section.GetRawText(), _SerializerOptions) ?? new T();
        }
        return new T();
    }

    private JsonElement LoadRawJson()
    {
        if (!File.Exists(FilePath))
            return JsonDocument.Parse("{}").RootElement;

        // Читаем файл с явным указанием UTF-8 кодировки
        var json = File.ReadAllText(FilePath, Encoding.UTF8);
        return JsonDocument.Parse(json).RootElement;
    }
}