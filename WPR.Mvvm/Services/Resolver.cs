using Microsoft.Extensions.DependencyInjection;
using WPR.Mvvm.Interfaces;

namespace WPR.Mvvm.Services;

internal class Resolver<T>(IServiceProvider services) : IResolver<T> where T : notnull
{
    public T Get() => services.GetRequiredService<T>();
}