namespace WPR.AppConfig.Implementations;

/// <summary>
/// Настройки в памяти
/// </summary>
internal class MemoryAppConfig<T> : IConfig<T> where T : new()
{

    private T? _Config;
    public Type ConfigType => typeof(T);
    object IConfigProvider.Config => Config!;

    public T Config => _Config ??= new T();

    public void Save()
    {
    }

    public void Reload() => _Config = new T();

}