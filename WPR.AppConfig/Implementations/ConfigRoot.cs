namespace WPR.AppConfig.Implementations;

/// <summary> Реализация агрегатора настроек </summary>
internal class ConfigRoot : IConfigRoot
{
    private readonly Dictionary<Type, IConfigProvider> _Configs = new();

    public IEnumerable<IConfigProvider> Providers => _Configs.Values;

    public T Get<T>() => GetConfig<T>().Config;

    private IConfig<T> GetConfig<T>()
    {
        if (_Configs.TryGetValue(typeof(T), out var value))
            return  (IConfig<T>) value;

        throw new InvalidOperationException($"Тип {typeof(T)} не зарегистрирован в настройках приложения");
    }

    public void Save<T>() => GetConfig<T>().Save();

    public void Reload<T>() => GetConfig<T>().Reload();



    /// <summary> Добавить поставщика настроек </summary>
    public void AddConfig<T>(IConfig<T>  config) => _Configs[typeof(T)] = config;
}