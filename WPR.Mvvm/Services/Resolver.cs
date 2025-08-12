using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;

namespace WPR.Mvvm.Services;

internal class Resolver<T>(IServiceProvider services) : IResolver<T> where T : notnull
{
    public T Get() => services.GetRequiredService<T>();

    public T Get(string name) => services.GetRequiredKeyedService<T>(name);

    public T Get(Action<T> configure)
    {
        var instance = Get();
        configure(instance);
        return instance;
    }
}